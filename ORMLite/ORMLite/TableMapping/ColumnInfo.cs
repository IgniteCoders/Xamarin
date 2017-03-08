using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
namespace ORMLite {
	public class ColumnInfo {

		public PropertyInfo property { get; private set; }

		public Type propertyType { get { return property.PropertyType; } }

		public String name {
			get {
				return property.Name;
			}
		}

		public ColumnInfo(PropertyInfo prop) {
			property = prop;
		}

		public bool IsPrimaryKey () {
			return name == Configuration.ID_COLUMN_NAME;
		}

		/* Return true if field is handled as primitive column in table*/
		public bool IsPrimitiveField {
			get {
				/*FieldInfo fieldInfo = property.DeclaringType.GetTypeInfo().GetDeclaredField(property.Name);
				if (fieldInfo.IsStatic) {
					return false;
				}*/
				if (propertyType == typeof(String) ||
					propertyType == typeof(string) ||
					propertyType == typeof(long) ||
					propertyType == typeof(double) ||
					propertyType == typeof(Double) ||
					propertyType == typeof(int) ||
					propertyType == typeof(bool) ||
					propertyType == typeof(Boolean) ||
					propertyType == typeof(byte[])) {
					return true;
				} else {
					return false;
				}
			}
		}

		/* Return true if field is handled as foreign key column in table*/
		public bool IsRelationship {
			get {
				return IsSingleRelationship || IsMultipleRelationship;
			}
		}

		/* Return true if field is a foreign key (hasOne, belongsTo)*/
		public bool IsSingleRelationship {
			get {
				if (typeof(PersistentEntity).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo())) {
					return true;
				} else {
					return false;
				}
			}
		}

		/* Return true if field is a foreign key (hasMany)*/
		public bool IsMultipleRelationship {
			get {
				if (typeof(IList).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()) && propertyType.GetTypeInfo().IsGenericType) {
					Type type = propertyType.GenericTypeArguments[0];
					if (typeof(PersistentEntity).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) {
						return true;
					} else {
						return false;
					}
				} else {
					return false;
				}
			}
		}

		public override string ToString() {
			return string.Format("[Column: {0} - {1}]", name, propertyType);
		}
	}
}
