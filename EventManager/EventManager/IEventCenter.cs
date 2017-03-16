using System;
using System.Collections.Generic;
namespace EventManager
{
	public interface IEventCenter {
		void SendEvent(String eventName, Dictionary<string, object> parameters);
		void SendEvent(Event eventToSend);
		void AddObserver(String eventName, IEventObserver observer);
		void RemoveObserver(IEventObserver observer);
		void AddReceiver(String eventName, EventReceiver receiver);
		void RemoveReceiver(EventReceiver receiver);
	}
}
