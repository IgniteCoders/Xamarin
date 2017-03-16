using System;
namespace EventManager {
	public abstract class EventReceiver {

		public IEventObserver observer;

		public abstract void DidReceiveEvent(Event receivedEvent);
	}
}
