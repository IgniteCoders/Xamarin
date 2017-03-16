using System;
using System.Collections.Generic;

namespace EventManager
{
	public class Event
	{
		public string name { get; set; }
		public Dictionary<string, object> parameters { get; set; }

		public Event(string name, Dictionary<string, object> parameters) {
			this.name = name;
			this.parameters = parameters;
		}
	}
}
