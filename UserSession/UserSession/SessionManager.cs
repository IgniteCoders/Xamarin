using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Linq;
using Xamarin.Forms;
namespace UserSession {
	public class SessionManager {

		private IUserPreferences userPreferences;

		// Sharedpref file name
		public const String PREFERENCES_NAME = "session";

		// All Shared Preferences Keys
		private const String IS_LOGIN = "is-login";
		private const String HAS_ROLE = "has-role";

		// User name (make variable public to access from outside)
		public const String KEY_IDENTIFIER = "identifier";
		public const String KEY_USERNAME = "username";
		public const String KEY_PASSWORD = "password";

		// Parámetros de GCM (GoogleCloudMessaging)
		public const String GCM_REGISTRATION_ID = "gcm-registration-id";
		public const String GCM_APP_VERSION = "gcm-app-version";
		public const String GCM_EXPIRATION_TIME = "on-server-expiration-time-ms";
		public const String GCM_USER = "gcm-user";

		public SessionManager() {
			userPreferences = DependencyService.Get<IUserPreferences>();
		}

		public static SessionManager GetSession() {
			return new SessionManager();
		}

		public static void CreateSession(String username, String password, int role) {
			SessionManager session = GetSession();
			session.SetLoggedIn(true);
			session.SetRole(role);
			session.SetUsername(username);
			session.SetPassword(password);
		}

		public static void RemoveSession() {
			SessionManager session = GetSession();
			session.Clear();
			session.SetLoggedIn(false);
		}

		public void Clear() {

		}

		/**
	     * Quick check for login
	     * **/
		// Get Login State
		public bool IsLoggedIn() {
			return userPreferences.GetBoolValue(IS_LOGIN, false);
		}

		public void SetLoggedIn(bool loggedIn) {
			userPreferences.PutBoolValue(IS_LOGIN, loggedIn);
		}

		public int GetRole() {
			return userPreferences.GetIntValue(HAS_ROLE, -1);
		}

		public void SetRole(int role) {
			userPreferences.PutIntValue(HAS_ROLE, role);
		}

		public void SetUsername(String username) {
			userPreferences.PutStringValue(KEY_USERNAME, username);
		}

		public String GetUsername() {
			return userPreferences.GetStringValue(KEY_USERNAME, "Anonymous");
		}

		public void SetPassword(String password) {
			userPreferences.PutStringValue(KEY_PASSWORD, password);
		}

		public String GetPassword() {
			return userPreferences.GetStringValue(KEY_PASSWORD, "");
		}

		public void SetIdentifier(long identifier) {
			userPreferences.PutLongValue(KEY_IDENTIFIER, identifier);
		}

		public long GetIdentifier() {
			return userPreferences.GetLongValue(KEY_IDENTIFIER, -1);
		}

		public void SetGCMRegistrationId(String regId) {
			userPreferences.PutStringValue(GCM_REGISTRATION_ID, regId);
		}

		public String GetGCMRegistrationId() {
			return userPreferences.GetStringValue(GCM_REGISTRATION_ID, "");
		}

		public void SetGCMAppVersion(int appVersion) {
			userPreferences.PutIntValue(GCM_APP_VERSION, appVersion);
		}

		public int GetGCMAppVersion() {
			return userPreferences.GetIntValue(GCM_APP_VERSION, Int32.MinValue);
		}

		public void SetGCMExpirationTime(long expirationTime) {
			userPreferences.PutLongValue(GCM_EXPIRATION_TIME, expirationTime);
		}

		public long GetGCMExpirationTime() {
			return userPreferences.GetLongValue(GCM_EXPIRATION_TIME, -1);
		}

		public void SetGCMUser(String user) {
			userPreferences.PutStringValue(GCM_USER, user);
		}

		public String GetGCMUser() {
			return userPreferences.GetStringValue(GCM_USER, "");
		}

	}
}
