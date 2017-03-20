using System;
using Android.Content;
using System.Collections.Generic;
using Xamarin.Forms;
using EventManager.Droid;

[assembly: Dependency(typeof(EventObserver))]

namespace EventManager.Droid {
	public class EventObserver : BroadcastReceiver, IEventObserver {

		public EventReceiver receiver;

		public EventObserver() {

		}

		public override void OnReceive(Context context, Intent intent) {
			Dictionary<string, object> userInfo = new Dictionary<string, object>();
			if (intent.Extras != null) {
				foreach (string key in intent.Extras.KeySet()) {
					userInfo[key] = intent.Extras.GetString(key);
				}
			}
			Event receivedEvent = new Event(intent.Action, userInfo);
			OnReceive(receivedEvent);
		}

		public void OnReceive(Event receivedEvent) {
			receiver.DidReceiveEvent(receivedEvent);
		}

		public void SetReceiver(EventReceiver receiver) {
			this.receiver = receiver;
			receiver.observer = this;
		}

		public EventReceiver GetReceiver() {
			return this.receiver;
		}
	}
}
