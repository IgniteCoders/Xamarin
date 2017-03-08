using System;
namespace ORMLite {
	public class Configuration {
		/** Set to true if you need to print the SQL queries in the console */
		public const bool SQL_CONSOLE_ENABLED = true;
		/** Defines the name of the identifier column in all tables */
		public const String ID_COLUMN_NAME = "id";
		/** The name of the database file that will be stored, it's must end with .db extension */
		public const String DATABASE_NAME = "networker.db";
		/** Number of the database version, should be increased on each database modification to recreate the database */
		public const int DATABASE_VERSION = 2;
		/** Define database create/update behavior; one of 'create', 'drop-create', 'update', '', */
		public const String DATABASE_CREATE = "drop-create";
		/** Name of the package where the domain classes are placed, if set to "", HQLite will search in all the application */
		public const String DOMAIN_PACKAGE = "Networker";
		/** Name of the static property where you will store your EntityManagers of the domain classes */
		public const String ACCESS_PROPERTY = "TABLE";
		/** Set the default fetch behaviour, true for "LAZY" and false for "EAGER" */
		public const bool LAZY_FETCH_TYPE = false;
	}
}
