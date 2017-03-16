using Xamarin.Forms;
using Foundation;
using UserSession.iOS;
[assembly: Dependency(typeof(UserPreferences))]

namespace UserSession.iOS {
	public static class UserSession {
		public static void Init() {
			DependencyService.Register<UserPreferences>();
		}
	}

	public class UserPreferences : IUserPreferences {

		NSUserDefaults preferences;

		public UserPreferences() {
			preferences = NSUserDefaults.StandardUserDefaults;
		}

		public void clear() {

		}

		public void PutStringValue(string key, string value) {
			preferences.SetString(value, key);
			preferences.Synchronize();
		}

		public void PutIntValue(string key, int value) {
			preferences.SetInt(value, key);
			preferences.Synchronize();
		}

		public void PutLongValue(string key, long value) {
			preferences.SetDouble(value, key);
			preferences.Synchronize();
		}

		public void PutFloatValue(string key, float value) {
			preferences.SetFloat(value, key);
			preferences.Synchronize();
		}

		public void PutBoolValue(string key, bool value) {
			preferences.SetBool(value, key);
			preferences.Synchronize();
		}

		public string GetStringValue(string key, string defaultValue) {
			return preferences.ValueForKey((NSString)key) != null ? preferences.StringForKey(key) : defaultValue;
		}

		public int GetIntValue(string key, int defaultValue) {
			return preferences.ValueForKey((NSString)key) != null ? (int)preferences.IntForKey(key) : defaultValue;
		}

		public long GetLongValue(string key, long defaultValue) {
			return preferences.ValueForKey((NSString)key) != null ? (long)preferences.DoubleForKey(key) : defaultValue;
		}

		public float GetFloatValue(string key, float defaultValue) {
			return preferences.ValueForKey((NSString)key) != null ? preferences.FloatForKey(key) : defaultValue;
		}

		public bool GetBoolValue(string key, bool defaultValue) {
			return preferences.ValueForKey((NSString)key) != null ? preferences.BoolForKey(key) : defaultValue;
		}
	}
}
