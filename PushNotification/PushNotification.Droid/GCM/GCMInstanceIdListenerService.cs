using Android.Content;
using Android.App;
using Android.Gms.Iid;

namespace PushNotification.Droid
{
	[Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
	public class GCMInstanceIDListenerService : InstanceIDListenerService
	{

		public override void OnTokenRefresh()
		{
			var intent = new Intent(this, typeof(GCMRegistrationIntentService));
			StartService(intent);
		}
	}
}