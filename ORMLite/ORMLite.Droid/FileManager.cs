using System;
using System.IO;
using ORMLite;
using ORMLite.Droid;
using Xamarin.Forms;
[assembly: Dependency (typeof (FileManager))]
namespace ORMLite.Droid {

	public static class ORMLite {
		public static void Init () {
			DependencyService.Register<FileManager>();
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
			return Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		}

		public string GetLocalFilePath (string fileName) {
			return Path.Combine (GetApplicationDirectoryPath(), fileName);
		}
	}
}
