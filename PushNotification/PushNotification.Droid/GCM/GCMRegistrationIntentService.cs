using Android.App;
using Android.Preferences;
using Android.Gms.Iid;
using Android;
using Android.Gms.Gcm;
using Android.Util;
using Android.Support.V4.Content;
using System;
using Android.Content;


namespace PushNotification.Droid
{
	[Service(Exported = false)]
	public class GCMRegistrationIntentService : IntentService
	{

		public static String SENDER_ID = "622828294913";

		public GCMRegistrationIntentService()
		{
		}

		protected override void OnHandleIntent(Intent intent)
		{
			Intent registrationComplete = new Intent(PushEventCenter.EVENT_NAMES.DEVICE_REGISTRATION_COMPLETED);
			try
			{
				InstanceID instanceID = InstanceID.GetInstance(this);
				String token = instanceID.GetToken(SENDER_ID, GoogleCloudMessaging.InstanceIdScope, null);
				registrationComplete.PutExtra(PushEventCenter.PARAMS_KEYS.DEVICE_TOKEN, token);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			LocalBroadcastManager.GetInstance(this).SendBroadcast(registrationComplete);
		}
	}
}