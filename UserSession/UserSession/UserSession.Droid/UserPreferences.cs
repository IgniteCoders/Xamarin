using Xamarin.Forms;
using System.Diagnostics;
using Android.Content;
using Android.Preferences;
using Java.Lang;
using UserSession.Droid;

[assembly: Dependency(typeof(UserPreferences))]

namespace UserSession.Droid {
	public static class UserSession {
		public static void Init() {
			DependencyService.Register<UserPreferences>();
		}
	}

	public class UserPreferences : IUserPreferences {

		// Shared preferences mode
		private const int PRIVATE_MODE = 0;
		private const int MODE_WORLD_READABLE = 2;

		ISharedPreferences preferences;
		ISharedPreferencesEditor editor;

		public UserPreferences() {
			Context context = Android.App.Application.Context;
			preferences = PreferenceManager.GetDefaultSharedPreferences(context);
			editor = preferences.Edit();
			try {
				preferences = context.GetSharedPreferences(SessionManager.PREFERENCES_NAME, PRIVATE_MODE);
				editor = preferences.Edit();
			} catch (NullPointerException e) {
				Debug.WriteLine(e.ToString());
			}
		}

		public void clear() {
			editor.Clear();
			editor.Commit();
		}

		public void PutStringValue(string key, string value) {
			editor.PutString(key, value);
			editor.Commit();
		}

		public void PutIntValue(string key, int value) {
			editor.PutInt(key, value);
			editor.Commit();
		}

		public void PutLongValue(string key, long value) {
			editor.PutLong(key, value);
			editor.Commit();
		}

		public void PutFloatValue(string key, float value) {
			editor.PutFloat(key, value);
			editor.Commit();
		}

		public void PutBoolValue(string key, bool value) {
			editor.PutBoolean(key, value);
			editor.Commit();
		}

		public string GetStringValue(string key, string defaultValue) {
			return preferences.GetString(key, defaultValue);
		}

		public int GetIntValue(string key, int defaultValue) {
			return preferences.GetInt(key, defaultValue);
		}

		public long GetLongValue(string key, long defaultValue) {
			return preferences.GetLong(key, defaultValue);
		}

		public float GetFloatValue(string key, float defaultValue) {
			return preferences.GetFloat(key, defaultValue);
		}

		public bool GetBoolValue(string key, bool defaultValue) {
			return preferences.GetBoolean(key, defaultValue);
		}
	}
}
