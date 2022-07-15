using System;
using System.Collections.Generic;

namespace Scrips.Agent {
	public class AgentEventHistoryManager {
		private string _name;
		private List<string> _listOfEvents;

		public AgentEventHistoryManager(string name) {
			_name = name;
			
			_listOfEvents = new List<string>();
		}

		public void AddHistoryEvent(string historyEvent) {
			string stringToAdd = _name + ": " + historyEvent + "\n";
			
			_listOfEvents.Add(stringToAdd);
		}

		public List<string> GetListOfEvents() {
			return _listOfEvents;
		}

		public string GetListOfEventsAsString() {
			return String.Join(String.Empty, _listOfEvents.ToArray());
		}

		public void Tick(int timeStep) {
			string stringToAdd = "\n" + "Time Step: " + timeStep + "\n"
			                     + "-----------------------" + "\n";
			_listOfEvents.Add(stringToAdd);
		}
	}
}