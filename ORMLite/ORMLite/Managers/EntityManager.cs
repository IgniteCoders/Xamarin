using System;
using System.Reflection;
using System.Collections.Generic;
using SQLite3Statement = SQLitePCL.sqlite3_stmt;
using SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace ORMLite {
	public class EntityManager<E> where E : PersistentEntity {
		private TableMapping<E> tableMapping = new TableMapping<E>();
		private DatabaseManager databaseManager;
		private SQLiteConnection database;


		public TableMapping<E> GetTableMapping() {
			return tableMapping;
		}

		/**
		 * Open a new database connection
		 */
		private void Open() {
			databaseManager = new DatabaseManager();
			database = databaseManager.GetDatabase();
		}

		/**
	     * Close the database connection
	     */
		private void Close() {
			database.Close();
		}

		/** Creates an object from the stored data in the cursor.
	     *
	     * @param cursor with the data of the query. It's should be pointing to the needed row.
	     * @return the generated instance.
	     */
		public E CursorToEntity(Cursor cursor) {
			try {
				E obj = (E)Activator.CreateInstance(tableMapping.type);

				obj.SetId(cursor.GetValue<long>(Configuration.ID_COLUMN_NAME));

				/*foreach (ColumnInfo column in this.tableMapping.columns) {
					//object value = typeof(Cursor).GetTypeInfo().GetDeclaredMethod("GetValue").MakeGenericMethod(column.propertyType).Invoke(cursor, new object[] { column.name });
					object value = cursor.GetValue(column.name, column.propertyType);	
					column.property.SetValue(obj, value);
				}*/

				for (int i = 1; i <= this.tableMapping.columns.Length; i ++) {
					ColumnInfo column = this.tableMapping.columns[i - 1];
					if (column.IsPrimitiveField) {
						object value = cursor.GetValue(i, column.propertyType);
						column.property.SetValue(obj, value);
					} else if (column.IsSingleRelationship) {
						object value = cursor.GetValue<long>(i);
						PersistentEntity relationObject = (PersistentEntity)Activator.CreateInstance(column.propertyType);
						if (Reflections.IsAttributePresent(column.property, typeof(BelongsTo)) ||
							(Reflections.IsAttributePresent(column.property, typeof(HasOne)) &&
							 ((HasOne)Reflections.GetAttribute(column.property, typeof(HasOne))).lazy)) {
							relationObject.SetServerId((long)value);
							column.property.SetValue(obj, relationObject);
						} else {
							value = relationObject.GetTableData().GetByServerId((dynamic)value);
							column.property.SetValue(obj, value);
						}
					} else if (column.IsMultipleRelationship) {
						if (Reflections.IsAttributePresent(column.property, typeof(HasMany))) {
							HasMany hasMany = (HasMany)Reflections.GetAttribute(column.property, typeof(HasMany));
							if (hasMany.lazy) {
								column.property.SetValue(obj, Activator.CreateInstance(column.propertyType));
							} else {
								Type relationType = column.propertyType.GenericTypeArguments[0];
								String columnName = "id" + hasMany.mappedBy.ToUpper().ToCharArray()[0] + hasMany.mappedBy.Substring(1);
								List<PersistentEntity> value = ((PersistentEntity)Activator.CreateInstance(relationType)).GetTableData().GetAllByField(columnName);
								column.property.SetValue(obj, value);
							}
						} else {
							throw new NotSupportedException("Use relationships not supported in lists, you must create an auxiliar table");
						}
					}
				}

				PersistentEntity superObject = GetSuperObject(obj.GetId());
				if (superObject != null) {
					Reflections.SetInstanceFromSuperInstance(obj, superObject);
				}

				return obj;
			} catch (Exception e) {
				SQLConsole.WriteLine(e.ToString());
				return null;
			}
		}

		/** Creates a <code>ContentValues</code> from the object.
	     *
	     * @param object to be converted in a <code>ContentValues</code> for persistent context
	     * @return the generated <code>ContentValues</code>.
	     */
		public Dictionary<String, object> GetContentValues(E obj) {
			Dictionary<String, object> values = new Dictionary<String, object>();
			if (obj.GetId() != -1) {
				values[Configuration.ID_COLUMN_NAME] = obj.GetId();
			}

			foreach (ColumnInfo column in this.tableMapping.columns) {
				if (!column.IsMultipleRelationship) {
					if (column.IsSingleRelationship) {
						if (Reflections.IsAttributePresent(column.property, typeof(BelongsTo)) || !Reflections.IsAttributePresent(column.property, typeof(HasOne))) {
							values[QueryGenerator<E>.ColumnName(column)] = ((PersistentEntity)column.property.GetValue(obj)).GetServerId();
						}
					} else {
						values[QueryGenerator<E>.ColumnName(column)] = column.property.GetValue(obj);
					}
				}
			}
			return values;
		}

		/** Creates an object from the stored data in the JSON.
	     *
	     * @param _JSONObject with the data of the object.
	     * @return the generated instance.
	     */
		public E Parse(JToken _JSONObject) {
			try {
				E obj = (E)Activator.CreateInstance(tableMapping.type);
				for (int i = 0; i < this.tableMapping.columns.Length; i++) {
					ColumnInfo column = this.tableMapping.columns[i];
					if (column.IsPrimitiveField) {
						if (_JSONObject[column.name] != null) {
							object value = column.propertyType == typeof(String) ? _JSONObject[column.name].Value<String>() : JsonConvert.DeserializeObject(_JSONObject[column.name].ToString(), column.propertyType);
							column.property.SetValue(obj, value);
						} else {
							column.property.SetValue(obj, null);
						}
					} else if (column.IsSingleRelationship) {
						object value = null;
						PersistentEntity relationObject = (PersistentEntity)Activator.CreateInstance(column.propertyType);

						if (_JSONObject[QueryGenerator<E>.ColumnName(column)] != null) {
							relationObject.SetServerId(JsonConvert.DeserializeObject<long>(_JSONObject[QueryGenerator<E>.ColumnName(column)].ToString()));
							value = relationObject;
						} else if (_JSONObject[column.property.Name] != null) {
							value = relationObject.GetTableData().Parse(_JSONObject[column.name]);
						}

						column.property.SetValue(obj, value);
					} else if (column.IsMultipleRelationship) {
						if (_JSONObject[column.name] != null) {
							PersistentEntity relationObject = (PersistentEntity)Activator.CreateInstance(column.propertyType.GenericTypeArguments[0]);
							List<PersistentEntity> value = relationObject.GetTableData().Parse(JArray.Parse(_JSONObject[column.name].ToString()));
							column.property.SetValue(obj, value);
						} else {
							column.property.SetValue(obj, null);
						}
					}
				}

				Type baseType = Reflections.GetBaseType(this.tableMapping.type);
				if (baseType != typeof(PersistentEntity)) {
					Reflections.SetInstanceFromSuperInstance(obj, ((PersistentEntity)Activator.CreateInstance(baseType)).GetTableData().Parse(_JSONObject));
				}
				return obj;
			} catch (Exception e) {
				SQLConsole.WriteLine(e.ToString());
				return null;
			}
		}


		/** Creates a list with objects from the stored data in the JSON.
	     *
	     * @param _JSONList with the data of the JSON array list.
	     * @return the generated list.
	     */
		public List<E> Parse(JArray _JSONList) {
			List<E> list = null;
			if (_JSONList != null && _JSONList.Count > 0) {
				list = new List<E>();
				foreach (JToken o in _JSONList) {
					list.Add(Parse(o));
				}
			}
			return list;
		}

		///** Creates a <code>JSONObject</code> from the object.
		// *
		// * @param object to be converted in a <code>JSONObject</code>.
		// * @return the generated <code>JSONObject</code>.
		// */
		//public JSONObject wrap(E object) {
		//	JSONObject _JSONObject = new JSONObject();
		//	try {
		//		Class superClass = domainClass.getSuperclass();
		//		PersistentEntity superObject = null;
		//		if (superClass != PersistentEntity.class) {
		//		                try {
		//		                    _JSONObject = ((PersistentEntity) superClass.newInstance()).getTableData().wrap(object);
		//		                } catch (Exception e) {
		//		                    e.printStackTrace();
		//		                }
		//		            }
		//		            for (Field field : getColumnFields()) {
		//		                _JSONObject = EntityFieldHelper.setFieldInJSONObject(object, field, _JSONObject);
		//		            }
		//		        } catch (Exception e) {
		//		            e.printStackTrace();
		//		        }
		//		        return _JSONObject;
		//		    }

		//		    /** Creates a <code>JSONArray</code> from the object.
		//		     *
		//		     * @param list to be converted in a <code>JSONArray</code>.
		//		     * @return the generated <code>JSONArray</code>.
		//		     */
		//		    public JSONArray wrap(List<E> list) {
		//	JSONArray _JSONArray = new JSONArray();
		//	if (list != null) {
		//		for (E object : list) {
		//			_JSONArray.put(wrap(object));
		//		}
		//	}
		//	return _JSONArray;
		//}

		private PersistentEntity GetSuperObject(long id) {
			Type superClass = Reflections.GetBaseType(tableMapping.type);
			PersistentEntity superObject = null;
			if (superClass != typeof(PersistentEntity)) {
				superObject = ((PersistentEntity)Activator.CreateInstance(superClass)).GetTableData().Get(id);
			}
			return superObject;
		}

		/**
	     * Convenience method for persisting a object into the database.
	     *
	     * @param object the object to persist in the database
	     * @throws SQLException
	     * @return the row ID of the newly persisted object, or -1 if an error occurred
	     */
		public long Insert(E obj) {
			Open();
			databaseManager.Execute(QueryGenerator<E>.InsertQuery(tableMapping, GetContentValues(obj)));
			long insertedId = databaseManager.InsertId();
			Close();
			return insertedId;
		}

		/**
		 * Convenience method for updating a object already inserted into the database.
		 *
		 * @param object the object to update in the database
		 * @throws SQLException
		 * @return the number of rows affected
		 */
		public int Update(E obj) {
			Open();
			int updatedRows = databaseManager.Execute(QueryGenerator<E>.UpdateQuery(tableMapping, GetContentValues(obj)));
			Close();
			return updatedRows;
		}

		/**
		 * Convenience method for removing objects from the database.
		 *
		 * @param object the object to remove from the database
		 * @return the number of rows affected, 1 if deleted correctly, 0 otherwise
		 */
		public int Delete(E obj) {
			Open();
			int deletedRows = databaseManager.Execute(QueryGenerator<E>.DeleteQuery(tableMapping, obj.GetId()));
			Close();
			return deletedRows;
		}

		//// Methods for retrieve rows from database
		public E Get(long id) {
			return GetByField(Configuration.ID_COLUMN_NAME, id);
		}

		public virtual E GetByServerId(long id) {
			return Get(id);
		}

		public E GetByField(String key, Object value) {
			Open();
			E obj = null;
			String sqlQuery = QueryGenerator<E>.SelectQuery(tableMapping, key + "='" + value.ToString() + "'");
			Cursor cursor = databaseManager.Select(sqlQuery);
			try {
				if (cursor.MoveToNext()) {
					obj = CursorToEntity(cursor);
				}
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
			} finally {
				try {
					cursor.Close();
				} catch (Exception e) {
					SQLConsole.WriteLine(e.StackTrace);
				}
				Close();
			}
			return obj;
		}

		public List<E> GetAll() {
			Open();
			List<E> listObjects = new List<E>();
			String sqlQuery = QueryGenerator<E>.SelectQuery(tableMapping, null);
			Cursor cursor = databaseManager.Select(sqlQuery);
			try {
				for (cursor.MoveToNext(); !cursor.IsAfterLast(); cursor.MoveToNext()) {
					E obj = CursorToEntity(cursor);
					listObjects.Add(obj);
				}
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
			} finally {
				try {
					cursor.Close();
				} catch (Exception e) {
					SQLConsole.WriteLine(e.StackTrace);
				}
				Close();
			}
			return listObjects;
		}

		public List<E> GetAllByField(String key, Object value) {
			Open();
			List<E> listObjects = new List<E>();
			String sqlQuery = QueryGenerator<E>.SelectQuery(tableMapping, key + "='" + value.ToString() + "'");
			Cursor cursor = databaseManager.Select(sqlQuery);
			try {
				for (cursor.MoveToNext(); !cursor.IsAfterLast(); cursor.MoveToNext()) {
					E obj = CursorToEntity(cursor);
					listObjects.Add(obj);
				}
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
			} finally {
				try {
					cursor.Close();
				} catch (Exception e) {
					SQLConsole.WriteLine(e.StackTrace);
				}
				Close();
			}
			return listObjects;
		}

		public List<E> GetAllByFieldInList(String key, Object[] values) {
			List<E> listObjects = new List<E>();
			String inClause = "(";
			for (int i = 0; i < values.Length; i++) {
				inClause += "'" + values[i].ToString() + "'";
				if (i != (values.Length - 1)) {
					inClause += ",";
				}
			}
			inClause += ")";

			Open();
			String sqlQuery = QueryGenerator<E>.SelectQuery(tableMapping, key + " in " + inClause);
			Cursor cursor = databaseManager.Select(sqlQuery);
			try {
				for (cursor.MoveToNext(); !cursor.IsAfterLast(); cursor.MoveToNext()) {
					E obj = CursorToEntity(cursor);
					listObjects.Add(obj);
				}
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
			} finally {
				try {
					cursor.Close();
				} catch (Exception e) {
					SQLConsole.WriteLine(e.StackTrace);
				}
				Close();
			}
			return listObjects;
		}

		public List<E> GetAllWithCriteria(CriteriaBuilder criteria) {
			List<E> listObjects = new List<E>();
			Open();
			String sqlQuery = QueryGenerator<E>.SelectQuery(tableMapping, criteria.Query(), criteria.GroupBy(), criteria.Having(), criteria.OrederBy(), criteria.Limit(), criteria.Distinct());
			SQLConsole.WriteLine(sqlQuery);
			Cursor cursor = databaseManager.Select(sqlQuery);
			try {
				for (cursor.MoveToNext(); !cursor.IsAfterLast(); cursor.MoveToNext()) {
					E obj = CursorToEntity(cursor);
					listObjects.Add(obj);
				}
			} catch (Exception e) {
				SQLConsole.WriteLine(e.StackTrace);
			} finally {
				try {
					cursor.Close();
				} catch (Exception e) {
					SQLConsole.WriteLine(e.StackTrace);
				}
				Close();
			}
			return listObjects;
		}

		public E GetWithCriteria(CriteriaBuilder criteria) {
			List<E> listObjects = GetAllWithCriteria(criteria);
			if (listObjects.Count > 0) {
				return listObjects[0];
			} else {
				return null;
			}
		}
	}
}
