using System;
namespace UserSession {
	public interface IUserPreferences {
		void PutStringValue(String key, string value);
		void PutIntValue(String key, int value);
		void PutLongValue(String key, long value);
		void PutFloatValue(String key, float value);
		void PutBoolValue(String key, bool value);

		String GetStringValue(String key, String defaultValue);
		int GetIntValue(String key, int defaultValue);
		long GetLongValue(String key, long defaultValue);
		float GetFloatValue(String key, float defaultValue);
		bool GetBoolValue(String key, bool defaultValue);
	}
}
