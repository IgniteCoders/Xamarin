using System;
using Xamarin.Forms;
using EventManager.iOS;
using Foundation;
using System.Collections.Generic;

[assembly: Dependency(typeof(EventCenter_iOS))]

namespace EventManager.iOS {
	
	public static class EventManager {
		public static void Init() {
			DependencyService.Register<EventCenter_iOS>();
			DependencyService.Register<EventObserver>();
		}
	}

	public class EventCenter_iOS : EventCenter {

		public override void SendEvent(string eventName, Dictionary<string, object> parameters) {
			SendEvent(new Event(eventName, parameters));
		}

		public override void SendEvent(Event eventToSend) {
			NSMutableDictionary<NSString, NSObject> userInfo = new NSMutableDictionary<NSString, NSObject>();
			foreach (string key in eventToSend.parameters.Keys) {
				//NSString key = (NSString)k.ToString();
				//NSString value = (NSString)eventToSend.parameters[key].ToString();
				userInfo[key] = NSObject.FromObject(eventToSend.parameters[key]);
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName(eventToSend.name, null, userInfo);
		}
		
		public override void AddObserver(string eventName, IEventObserver observer) {
			((EventObserver)observer).token = NSNotificationCenter.DefaultCenter.AddObserver((NSString)eventName, ((EventObserver)observer).ReportEvent);
		}

		public override void RemoveObserver(IEventObserver observer) {
			NSNotificationCenter.DefaultCenter.RemoveObserver(((EventObserver)observer).token);
		}
	}
}
