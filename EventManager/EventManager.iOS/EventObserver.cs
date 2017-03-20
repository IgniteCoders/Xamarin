using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Foundation;
using EventManager.iOS;

[assembly: Dependency(typeof(EventObserver))]

namespace EventManager.iOS {
	public class EventObserver : IEventObserver {

		public EventReceiver receiver;
		public NSObject token;

		public EventObserver() {

		}

		public void ReportEvent(NSNotification notification) {
			Dictionary<string, object> userInfo = new Dictionary<string, object>();
			if (notification.UserInfo != null) {
				foreach (NSObject key in notification.UserInfo.Keys) {
					userInfo[key.ToString()] = notification.UserInfo[key];
				}
			}
			Event receivedEvent = new Event(notification.Name, userInfo);
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
