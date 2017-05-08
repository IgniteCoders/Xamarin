using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Android.Support.V4.Content;

namespace PushNotification.Droid
{
	[Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GCMNotificationIntentService : IntentService
	{
		public GCMNotificationIntentService() { }

		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				HandlePushNotification(ApplicationContext, intent);
			}
			finally
			{
				WakefulReceiver.CompleteWakefulIntent(intent);
			}
		}


		private void HandlePushNotification(Context context, Intent intent)
		{
			Intent notificationReceived = new Intent(PushEventCenter.EVENT_NAMES.PUSH_NOTIFICATION_RECEIVED);
			foreach (String key in intent.Extras.KeySet())
			{
				notificationReceived.PutExtra(key, intent.Extras.GetString(key));
			}
			LocalBroadcastManager.GetInstance(this).SendBroadcast(notificationReceived);
		}
	}
}