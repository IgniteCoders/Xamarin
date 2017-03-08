using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
namespace ORMLite {
	public class TableMapping<E> where E : PersistentEntity {
		public Type type { get { return typeof(E); } }

		public String name { get { return type.Name; } }

		public ColumnInfo[] columns { get; private set; }

		public TableMapping() {

			List<PropertyInfo> properties = Reflections.GetDeclaredFields (type);
			List<ColumnInfo> cols = new List<ColumnInfo>();
			foreach (PropertyInfo property in properties) {
				var ignore = false;//property.GetCustomAttributes(typeof(IgnoreAttribute), true).Count() > 0;
				if (property.CanWrite && !ignore) {
					cols.Add(new ColumnInfo(property));
				}
			}
			columns = cols.ToArray();
		}







		public ColumnInfo FindColumn(string columnName) {
			var exact = columns.FirstOrDefault(c => c.name.ToLower() == columnName.ToLower());
			return exact;
		}
	}
}
