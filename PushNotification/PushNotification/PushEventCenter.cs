using System;
namespace PushNotification
{
	public class PushEventCenter
	{

		public static class EVENT_NAMES
		{
			public static String DEVICE_REGISTRATION_COMPLETED = "device_registration_completed";
			public static String PUSH_NOTIFICATION_RECEIVED = "push_notification_received";
		}

		public static class PARAMS_KEYS
		{
			public static String DEVICE_TOKEN = "device_token";
		}

		public PushEventCenter()
		{
		}
	}
}
