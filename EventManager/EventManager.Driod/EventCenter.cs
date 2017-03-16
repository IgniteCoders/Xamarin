using System;
using Xamarin.Forms;
using EventManager.Droid;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Android.Text;

[assembly: Dependency(typeof(EventCenter_Droid))]

namespace EventManager.Droid {
	
	public static class EventManager {
		public static void Init() {
			DependencyService.Register<EventCenter_Droid>();
			DependencyService.Register<EventObserver>();
		}
	}

	public class EventCenter_Droid : EventCenter {
		
		Context context = Android.App.Application.Context;

		public override void SendEvent(string eventName, Dictionary<string, object> parameters) {
			SendEvent(new Event(eventName, parameters));
		}

		public override void SendEvent(Event eventToSend) {
			Intent intent = new Intent(eventToSend.name);

			foreach (string key in eventToSend.parameters.Keys) {
				intent.PutExtra(key, eventToSend.parameters[key].ToString());
			}

			LocalBroadcastManager.GetInstance(context).SendBroadcast(intent);
		}
		
		public override void AddObserver(string eventName, IEventObserver observer) {
			IntentFilter intent = new IntentFilter(eventName);
			LocalBroadcastManager.GetInstance(context).RegisterReceiver((EventObserver)observer, intent);
		}

		public override void RemoveObserver(IEventObserver observer) {
			LocalBroadcastManager.GetInstance(context).UnregisterReceiver((EventObserver)observer);
		}
	}
}
