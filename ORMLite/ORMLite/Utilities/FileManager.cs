using System;
using System.Threading.Tasks;
namespace ORMLite {
	/// <summary>
	/// Define an API for loading and saving a text file. Reference this interface
	/// in the common code, and implement this interface in the app projects for
	/// iOS, Android and WinPhone. Remember to use the 
	///     [assembly: Dependency (typeof (SaveAndLoad_IMPLEMENTATION_CLASSNAME))]
	/// attribute on each of the implementations.
	/// </summary>
	public interface FileManager {
		String GetApplicationDirectoryPath();
		String GetLocalFilePath(String filename);
		bool FileExists(String filename);
		String CreatePathToFile(String fileName);

		Task SaveTextAsync(string filename, string text);
		Task<string> LoadTextAsync(string filename);
	}
}
