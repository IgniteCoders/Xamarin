
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.Content;

namespace Reachability.Droid {
	[BroadcastReceiver(Enabled = true)]
	//[IntentFilter(new[] { "android.net.conn.CONNECTIVITY_CHANGE" })]
	[Preserve(AllMembers = true)]
	public class ReachabilityEventReceiver : BroadcastReceiver {
		public override void OnReceive(Context context, Intent intent) {
			Intent i = new Intent(NetworkManager.EVENT_NAME);
			//intent.PutExtra(key, eventToSend.parameters[key].ToString());
			LocalBroadcastManager.GetInstance(context).SendBroadcast(i);
		}
	}
}
