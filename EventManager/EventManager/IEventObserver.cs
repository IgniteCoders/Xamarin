using System;
namespace EventManager {
	public interface IEventObserver {
		void OnReceive(Event receivedEvent);
		void SetReceiver(EventReceiver receiver);
		EventReceiver GetReceiver();
	}
}
