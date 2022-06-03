using System.Collections.Generic;

namespace Scrips.Agent {
	public class AgentEventHistoryManager {
		private List<string> _listOfEvents;

		public AgentEventHistoryManager() {
			_listOfEvents = new List<string>();
		}

		public void AddHistoryEvent(string historyEvent) {
			_listOfEvents.Add(historyEvent);
		}

		public List<string> GetListOfEvents() {
			return _listOfEvents;
		}

		public string GetListOfEventsAsString() {
			string s = "";

			foreach (string historyEvent in _listOfEvents) {
				s += historyEvent + "\n";
			}

			return s;
		}
	}
}