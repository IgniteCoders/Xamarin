using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace EventManager {
	public abstract class EventCenter : IEventCenter {

		private static EventCenter instance;

		//returning the reference
		public static EventCenter Default() {
			if (instance == null)
				instance = DependencyService.Get<IEventCenter>() as EventCenter;
			return instance;
		}

		public EventCenter() {
			
		}

		public void AddReceiver(string eventName, EventReceiver receiver) {
			IEventObserver observer = DependencyService.Get<IEventObserver>();
			observer.SetReceiver(receiver);
			AddObserver(eventName, observer);
		}

		public void RemoveReceiver(EventReceiver receiver) {
			IEventObserver observer = receiver.observer;
			RemoveObserver(observer);
		}

		public abstract void AddObserver(string eventName, IEventObserver observer);

		public abstract void RemoveObserver(IEventObserver observer);

		public abstract void SendEvent(Event eventToSend);

		public abstract void SendEvent(string eventName, Dictionary<string, object> parameters);
	}
}
