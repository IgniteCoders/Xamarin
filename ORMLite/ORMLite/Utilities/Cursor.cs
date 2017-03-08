using System;
using SQLite3Statement = SQLitePCL.sqlite3_stmt;
using SQLite;
using SQLitePCL;
namespace ORMLite {
	public class Cursor {

		private SQLite3Statement statement;
		private String query;
		private SQLiteConnection database;
		private SQLite3.Result result = SQLite3.Result.Empty;

		public Cursor(SQLiteConnection database, String q) {
			this.database = database;
			this.query = q;
		}

		public void Open() {
			statement = SQLite3.Prepare2(database.Handle, query);
		}

		public void Close() {
			result = SQLite3.Finalize(statement);
		}

		public bool MoveToNext() {
			result = SQLite3.Step(statement);
			return result == SQLite3.Result.Row;
		}

		/*public bool MoveToFirst() {
			result = SQLite3.Reset(statement);
			return result == SQLite3.Result.Row;
		}*/

		public bool IsAfterLast() {
			return result == SQLite3.Result.Done;
		}

		public object GetValue(int columnIndex, Type propertyType) {
			object value = null;
			if (propertyType == typeof(String) || propertyType == typeof(string)) {
				value = SQLite3.ColumnString(statement, columnIndex);
			} else if (propertyType == typeof(long)) {
				value = SQLite3.ColumnInt64(statement, columnIndex);
			} else if (propertyType == typeof(Single) || propertyType == typeof(Double) || propertyType == typeof(Decimal)) {
				value = SQLite3.ColumnDouble(statement, columnIndex);
			} else if (propertyType == typeof(int)) {
				value = SQLite3.ColumnInt(statement, columnIndex);
			} else if (propertyType == typeof(bool) || propertyType == typeof(Boolean)) {
				value = (SQLite3.ColumnInt(statement, columnIndex) != 0);
			} else if (propertyType == typeof(byte[])) {
				value = SQLite3.ColumnByteArray(statement, columnIndex);
			}/* else if (column.isSingleRelationship) {
				value = (V)(object)SQLite3.ColumnString(statement, columnIndex);
			}*/
			return value;
		}

		public object GetValue(String columnName, Type propertyType) {
			return GetValue(ColumnIndex(columnName), propertyType);
		}

		public V GetValue<V>(int columnIndex) {
			return (V)GetValue(columnIndex, typeof(V));
		}

		public V GetValue<V>(String columnName) {
			return GetValue<V>(ColumnIndex(columnName));
		}

		public int ColumnIndex(String columnName) {
			return SQLite3.BindParameterIndex(statement, columnName);
		}
	}
}
