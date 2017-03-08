using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace ORMLite {

	public static class QueryGenerator<E> where E : PersistentEntity {
		
		private static Regex limitPattern = new Regex (@"\s*\d+\s*(,\s*\d+\s*)?");

		/******************************************************** CRUD queries */
		public static String InsertQuery(TableMapping<E> table, Dictionary<String, Object> contentValues) {
			bool ignoreId = !contentValues.ContainsKey(Configuration.ID_COLUMN_NAME);
			String sqlQuery = "INSERT INTO " + table.name + " (" + (ignoreId ? "" : Configuration.ID_COLUMN_NAME);
			String sqlValues = "VALUES (" + (ignoreId ? "" : contentValues[Configuration.ID_COLUMN_NAME]);
			for (int i = 0; i < table.columns.Length; i++) {
				ColumnInfo field = table.columns[i];
				String columnName = ColumnName(field);
				if (!field.IsMultipleRelationship && contentValues.ContainsKey(columnName)) {
					if (i > 0) {
						sqlQuery += ", ";
						sqlValues += ", ";
					}
					sqlQuery += columnName;
					if (contentValues[columnName] != null) {
						sqlValues += "'" + contentValues[columnName]?.ToString() + "'";
					} else {
						sqlValues += "NULL";
					}
				}
			}
			sqlQuery += ") " + sqlValues + ")";
			return sqlQuery;
		}

		public static String UpdateQuery(TableMapping<E> table, Dictionary<String, Object> contentValues) {
			String sqlQuery = "UPDATE " + table.name + " SET ";
			for (int i = 0; i < table.columns.Length; i++) {
				ColumnInfo field = table.columns[i];
				if (!field.IsPrimaryKey() && !field.IsMultipleRelationship) {
					String columnName = ColumnName (field);
					if (i > 0) { sqlQuery += ", "; }
					if (contentValues[columnName] != null) {
						sqlQuery += columnName + " = '" + contentValues[columnName]?.ToString() + "'";
					} else {
						sqlQuery += columnName + " = NULL";
					}
				}
			}
			sqlQuery += " WHERE " + Configuration.ID_COLUMN_NAME + "=" + contentValues[Configuration.ID_COLUMN_NAME];
			return sqlQuery;
		}

		public static String DeleteQuery(TableMapping<E> table, long id) {
			return "DELETE FROM " + table.name + " WHERE " + Configuration.ID_COLUMN_NAME + "=" + id;
		}

		public static String SelectQuery (TableMapping<E> table, String where) {
			return SelectQuery (table, where, null, null, null, null, false);
		}

		/**
	     * Build an SQL query string from the given clauses.
	     *
	     * @param distinct true if you want each row to be unique, false otherwise.
	     * @param tables The table names to compile the query against.
	     * @param columns A list of which columns to return. Passing null will
	     *            return all columns, which is discouraged to prevent reading
	     *            data from storage that isn't going to be used.
	     * @param where A filter declaring which rows to return, formatted as an SQL
	     *            WHERE clause (excluding the WHERE itself). Passing null will
	     *            return all rows for the given URL.
	     * @param groupBy A filter declaring how to group rows, formatted as an SQL
	     *            GROUP BY clause (excluding the GROUP BY itself). Passing null
	     *            will cause the rows to not be grouped.
	     * @param having A filter declare which row groups to include in the cursor,
	     *            if row grouping is being used, formatted as an SQL HAVING
	     *            clause (excluding the HAVING itself). Passing null will cause
	     *            all row groups to be included, and is required when row
	     *            grouping is not being used.
	     * @param orderBy How to order the rows, formatted as an SQL ORDER BY clause
	     *            (excluding the ORDER BY itself). Passing null will use the
	     *            default sort order, which may be unordered.
	     * @param limit Limits the number of rows returned by the query,
	     *            formatted as LIMIT clause. Passing null denotes no LIMIT clause.
	     * @return the SQL query string
	     */
		public static String SelectQuery(TableMapping<E> table, String where, String groupBy, 
		                                    String having, String orderBy, String limit, bool distinct) {
			if (String.IsNullOrEmpty (groupBy) && !String.IsNullOrEmpty (having)) {
				throw new ArgumentException ("HAVING clauses are only permitted when using a groupBy clause");
			}
			if (!String.IsNullOrEmpty (limit) && !limitPattern.Match (limit).Success) {
				throw new ArgumentException ("invalid LIMIT clauses:" + limit);
			}

			StringBuilder sqlQuery = new StringBuilder();
			sqlQuery.Append ("SELECT " + Configuration.ID_COLUMN_NAME);

			if (distinct) {
				sqlQuery.Append("DISTINCT ");
			}
			if (table.columns != null && table.columns.Length != 0) {
				for (int i = 0; i < table.columns.Length; i++) {
					ColumnInfo field = table.columns[i];
					//if (i > 0) { sqlQuery.Append (", "); }
					if (!field.IsMultipleRelationship) {
						String columnName = ColumnName(field);
						sqlQuery.Append(", " + columnName);
					}
				}
			}/* else {
				sqlQuery.Append ("* ");
			}*/
			sqlQuery.Append (" FROM " + table.name);
			AppendClause (sqlQuery, "WHERE", where);
			AppendClause (sqlQuery, "GROUP BY", groupBy);
			AppendClause (sqlQuery, "HAVING", having);
			AppendClause (sqlQuery, "ORDER BY", orderBy);
			AppendClause (sqlQuery, "LIMIT", limit);

			return sqlQuery.ToString();
		}

		private static void AppendClause (StringBuilder sqlQuery, String name, String clause) {
			if (!String.IsNullOrEmpty (clause)) {
				sqlQuery.Append (" " + name + " " + clause);
			}
		}




		/******************************************************** Table queries */

		/**
		 * Generate the <code>CREATE TABLE<code> query from a <code>PersistentEntity</code> class
		 * @param domainClass the class of the persistent entity, must extends <code>PersistentEntity</code>
		 * @return the generated <code>CREATE TABLE<code> query
		 */
		public static String CreateTableQuery(TableMapping<E> table) {
			ColumnInfo[] columns = table.columns;
			String sqlCreate = "CREATE TABLE IF NOT EXISTS " + table.name + " (" + Configuration.ID_COLUMN_NAME + " INTEGER PRIMARY KEY AUTOINCREMENT";
			Type superClass = Reflections.GetBaseType(table.type);
			/*if (superClass != typeof(PersistentEntity)) {
				//TODO: hay que ver en que orden se crean las clases y como corregir que la referencia no existe
				//TODO: en principio esta resuelto con foreign_check = O;
				sqlCreate += " REFERENCES " + superClass.Name + "(" + Configuration.ID_COLUMN_NAME + ") ON DELETE CASCADE";
				//TODO: esto lo hice por ON DELETE CASCADE pero no funciona :S
			}*/
			for (int i = 0; i < columns.Length; i++) {
				ColumnInfo column = columns[i];
				if (!column.IsMultipleRelationship) {
					sqlCreate += ", " + ColumnDeclaration(column);
				}
			}
			sqlCreate += ");";
			//        TODO: Cuidado con estos replace que son solo esteticos y pueden joder la sentencia
			return sqlCreate.Replace("  ", " ").Replace(" ,", ",");
		}

		/**
		 * Generate the <code>DROP TABLE<code> query from a <code>PersistentEntity</code> class
		 * @param domainClass the class of the persistent entity, must extends <code>PersistentEntity</code>
		 * @return the generated <code>DROP TABLE<code> query
		 */
		public static String DropTableQuery(TableMapping<E> table) {
			return "DROP TABLE IF EXISTS " + table.name;
		}

		/******************************************************** Column queries */

		/**
		 * Generate the column declaration for SQL from a <code>Field</code>
		 * @param field the field of the persistent entity
		 * @return the column declaration as "field name" + "field type" + "field constraints"
		 */
		private static String ColumnDeclaration(ColumnInfo column) {
			return (ColumnName(column) + " " + ColumnType(column) + " " + ColumnConstraints(column));
		}

		/**
		 * Generate the column name for SQL from a <code>Field</code>
		 * @param field the field of the persistent entity
		 * @return the column name. If the field is a single relationship, this method returns "idField" instead of "field"
		 */
		public static String ColumnName(ColumnInfo column) {
			if (column.IsPrimitiveField) {
				return column.name;
			} else if (column.IsSingleRelationship) {
				return "id" + column.name.Substring(0, 1).ToUpper() + column.name.Substring(1);
			} else {
				return column.name;
			}
		}

		/**
		 * Generate a collection with the columns names for SQL from a <code>PersistentEntity</code>
		 * @param domainClass the class of the persistent entity, must extends <code>PersistentEntity</code>
		 * @return List of columns names
		 */

		/*public static List<String> getColumnNames(Class<PersistentEntity> domainClass) {
			List<Field> fields = EntityFieldHelper.getColumnFields(domainClass);
			List<String> columns = new ArrayList<String>();
			columns.add(Configuration.ID_COLUMN_NAME);
			for (Field field : fields) {
				if (!EntityFieldHelper.isCollectionRelationField(field)) {
					columns.add(getColumnName(field));
				}
			}
			return columns;
		}*/

		/**
		 * Generate the column type for SQL from a <code>Field</code> type
		 * @param field the field of the persistent entity
		 * @return the column type. If the field is a single relationship, this method returns "LONG" to store the id
		 */
		public static String ColumnType(ColumnInfo column) {
			String result = "";
			Type propertyType = column.propertyType;
			if (propertyType == typeof(String) || propertyType == typeof(string)) {
				result += "TEXT";
			} else if (propertyType == typeof(long)) {
				result += "LONG";
			} else if (propertyType == typeof(Single) || propertyType == typeof(Double) || propertyType == typeof(Decimal)) {
				result += "DOUBLE";
			} else if (propertyType == typeof(int)) {
				result += "INTEGER";
			} else if (propertyType == typeof(bool) || propertyType == typeof(Boolean)) {
				result += "BOOLEAN";
			} else if (propertyType == typeof(byte[])) {
				result += "BLOB";
			} else if (column.IsSingleRelationship) {
				result += "LONG";
			}
			return result;
		}

		/**
	     * Generate the column constraints for SQL from a <code>Field</code> if <code>Constraint</code> annotation is present
	     * @param field the field of the persistent entity
	     * @return the column constraints
	     */
		private static String ColumnConstraints(ColumnInfo column) {
			String result = "";
			if (Reflections.IsAttributePresent(column.property, typeof(Constraints))) {
			Constraints constraints = (Constraints)Reflections.GetAttribute(column.property, typeof(Constraints));
	            if (constraints.unique == true) {
	                result += " UNIQUE";
	            }
	            if (constraints.nullable == false) {
	                result += " NOT NULL";
	            }
	        }
			//        if (EntityFieldHelper.isSingleRelationField(field) && !field.isAnnotationPresent(HasOne.class)) {
			//            result += " REFERENCES " + getTableName((Class<PersistentEntity>) field.getType()) + "(" + Configuration.ID_COLUMN_NAME + ")";
			//            if (field.isAnnotationPresent(BelongsTo.class)) {
			//                result += " ON DELETE CASCADE";
			//            }
			//        }
			return result;
		}
	}
}
