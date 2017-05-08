using System;
using System.IO;
using ORMLite;
using ORMLite.iOS;
using Xamarin.Forms;
[assembly: Dependency (typeof (FileManager))]
namespace ORMLite.iOS {

	public static class ORMLite {
		public static void Init () {
			DependencyService.Register<FileManager> ();
		}
	}

	public class FileManager : IFileManager {
		public FileManager () {
			
		}

		public string CreatePathToFile (string fileName) {
			throw new NotImplementedException ();
		}

		public bool FileExists (string fileName) {
			return File.Exists (fileName);
		}

		public string GetApplicationDirectoryPath () {
			string docFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string libFolder = Path.Combine (docFolder, "..", "Library");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return libFolder;
		}

		public string GetLocalFilePath (string fileName) {
			return Path.Combine (GetApplicationDirectoryPath(), fileName);
		}
	}
}
