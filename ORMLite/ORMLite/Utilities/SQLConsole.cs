using System;
using System.Diagnostics;
namespace ORMLite {
	public static class SQLConsole {
		public static void WriteLine(String message) {
			if (Configuration.SQL_CONSOLE_ENABLED) {
				Debug.WriteLine(message);
			}
		}
	}
}
