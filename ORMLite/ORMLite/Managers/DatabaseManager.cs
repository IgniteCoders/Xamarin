using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using SQLite;
namespace ORMLite {
	public class DatabaseManager {

		//readonly SQLiteAsyncConnection database;
		private String dbPath;
		private SQLiteConnection database;

		public DatabaseManager() {
			IFileManager fileManager = DependencyService.Get<IFileManager>();
			dbPath = fileManager.GetLocalFilePath(Configuration.DATABASE_NAME);
			if (fileManager.FileExists(dbPath)) {
				database = GetDatabase();
				//database.Trace = true;
				try {
					int version = database.ExecuteScalar<int>("PRAGMA user_version;");
					if (Configuration.DATABASE_VERSION > version) {
						OnUpgrade(database, version, Configuration.DATABASE_VERSION);
						database.Execute("PRAGMA user_version = " + Configuration.DATABASE_VERSION + ";");
					}
				} catch (SQLiteException e) {
					Debug.WriteLine(e.ToString());
				} finally {
					database.Close();
				}
			} else {
				database = GetDatabase();
				//database.Trace = true;
				try {
					OnCreate(database);
					database.Execute("PRAGMA user_version = " + Configuration.DATABASE_VERSION + ";");
				} catch (SQLiteException e) {
					Debug.WriteLine(e.ToString());
				} finally {
					database.Close();
				}
			}
		}

		public Cursor Select(String query) {
			SQLConsole.WriteLine(query);
			Cursor cursor = new Cursor(this.GetDatabase(), query);
			cursor.Open();
			return cursor;
		}

		public int Execute(String query) {
			SQLConsole.WriteLine(query);
			int result = database.Execute(query);
			return result;
		}

		public long InsertId() {
			return SQLite3.LastInsertRowid(database.Handle);
		}

		public SQLiteConnection GetDatabase(bool withLock = false) {
			//if (database == null) {
			if (withLock == true) {
				database = new SQLiteConnectionWithLock(new SQLiteConnectionString(dbPath, true), SQLiteOpenFlags.ReadWrite);
			} else {
				database = new SQLiteConnection(dbPath);
			}
			//}
			return database;
		}

		public SQLiteAsyncConnection GetDatabaseAsync() {
			return new SQLiteAsyncConnection(dbPath);
		}

		/*public SQLiteAsyncConnection GetDatabaseAsync() {
			return new SQLiteAsyncConnection(dbPath);
		}*/

		public void OnCreate(SQLiteConnection database) {
			SQLConsole.WriteLine("Creating database file...");
			SQLConsole.WriteLine(" - Name: " + Configuration.DATABASE_NAME);
			SQLConsole.WriteLine(" - Version: " + Configuration.DATABASE_VERSION);
			SQLConsole.WriteLine(" - Create: " + Configuration.DATABASE_CREATE);
			if (Configuration.DATABASE_CREATE.Equals("create") || Configuration.DATABASE_CREATE.Equals("drop-create")) {
				database.Execute("PRAGMA FOREIGN_KEYS = OFF");//validateDatabase(database);
				try {
					List<TypeInfo> types = Reflections.GetTypesFromAssembly(Configuration.DOMAIN_PACKAGE);
					foreach (TypeInfo type in types) {
						if (type.IsSubclassOf(typeof(PersistentEntity))) {
							PersistentEntity entity = (PersistentEntity)Activator.CreateInstance(type.AsType());

							//String query = (String)typeof(QueryGenerator<PersistentEntity>).GetTypeInfo().MakeGenericType(type.AsType()).GetTypeInfo().GetDeclaredMethod("CreateTableQuery").Invoke(null, new object[] { entity.GetTableData() });
							//Execute(QueryGenerator<PersistentEntity>.CreateTableQuery((entity.GetTableData<PersistentEntity>()).GetTableMapping()));
							dynamic tableData = entity.GetTableData();//GetType().GetTypeInfo().GetDeclaredMethod("GetTableData").MakeGenericMethod(entity.GetType()).Invoke(entity, null);
																	  //Reflections.CastTo();

							String query = (String)typeof(QueryGenerator<>).GetTypeInfo().MakeGenericType(type.AsType()).GetTypeInfo().GetDeclaredMethod("CreateTableQuery").Invoke(null, new object[] { tableData.GetTableMapping() });
							//QueryGenerator<PersistentEntity>.CreateTableQuery(tableData);
							//dynamic tableData = (EntityManager<PersistentEntity>)Convert.ChangeType(entity.GetType().GetTypeInfo().GetDeclaredMethod("GetTableData").MakeGenericMethod(entity.GetType()).Invoke(entity, null), typeof(EntityManager<PersistentEntity>));
							Execute(query);
						}
					}
				} catch (System.IO.FileNotFoundException e) {
					Debug.WriteLine(e.ToString());
				}
				database.Execute("PRAGMA FOREIGN_KEYS = ON");
			}
		}

		public void OnUpgrade(SQLiteConnection database, int oldVersion, int newVersion) {
			SQLConsole.WriteLine("Updating database file...");
			SQLConsole.WriteLine(" - Name: " + Configuration.DATABASE_NAME);
			SQLConsole.WriteLine(" - Version: " + oldVersion);

			if (Configuration.DATABASE_CREATE.Equals("drop-create")) {
				database.Execute("PRAGMA FOREIGN_KEYS = OFF");
				try {
					List<TypeInfo> types = Reflections.GetTypesFromAssembly(Configuration.DOMAIN_PACKAGE);
					foreach (TypeInfo type in types) {
						if (type.IsSubclassOf(typeof(PersistentEntity))) {
							PersistentEntity entity = (PersistentEntity)Activator.CreateInstance(type.AsType());

							//String query = (String)typeof(QueryGenerator<PersistentEntity>).GetTypeInfo().MakeGenericType(type.AsType()).GetTypeInfo().GetDeclaredMethod("CreateTableQuery").Invoke(null, new object[] { entity.GetTableData() });
							//Execute(QueryGenerator<PersistentEntity>.CreateTableQuery((entity.GetTableData<PersistentEntity>()).GetTableMapping()));
							dynamic tableData = entity.GetTableData();//GetType().GetTypeInfo().GetDeclaredMethod("GetTableData").MakeGenericMethod(entity.GetType()).Invoke(entity, null);
																	  //Reflections.CastTo();

							String query = (String)typeof(QueryGenerator<>).GetTypeInfo().MakeGenericType(type.AsType()).GetTypeInfo().GetDeclaredMethod("DropTableQuery").Invoke(null, new object[] { tableData.GetTableMapping() });
							//QueryGenerator<PersistentEntity>.CreateTableQuery(tableData);
							//dynamic tableData = (EntityManager<PersistentEntity>)Convert.ChangeType(entity.GetType().GetTypeInfo().GetDeclaredMethod("GetTableData").MakeGenericMethod(entity.GetType()).Invoke(entity, null), typeof(EntityManager<PersistentEntity>));
							Execute(query);
						}
					}
				} catch (System.IO.FileNotFoundException e) {
					Debug.WriteLine(e.ToString());
				}

			} else if (Configuration.DATABASE_CREATE.Equals("update")) {
				database.Execute("PRAGMA FOREIGN_KEYS = OFF");
				/*ArrayList<Table> newTables = getDomainStructure();
				ArrayList<Table> oldTables = getDatabaseStructure(database);
				for (Table oldTable : oldTables) {
					if (!newTables.contains(oldTable)) {
						String dropTableQuery = "DROP TABLE IF EXISTS " + oldTable.name;
						SQLConsole.Log(dropTableQuery);
						database.execSQL(dropTableQuery);
					} else {
						// TODO: comprobar campos
					}
				}
				for (Table newTable : newTables) {
					if (!oldTables.contains(newTable)) {
						String createTableQuery = SQLQueryGenerator.getCreateTable(newTable.type);
						SQLConsole.Log(createTableQuery);
						database.execSQL(createTableQuery);
					}
				}*/
				database.Execute("PRAGMA FOREIGN_KEYS = ON");

				//validateDatabase(database);
			}

			OnCreate(database);
		}

		public void ClearDatabase() {
			SQLiteConnection database = GetDatabase();
			OnUpgrade(database, 0, Configuration.DATABASE_VERSION);
			database.Close();
		}
	}
}
