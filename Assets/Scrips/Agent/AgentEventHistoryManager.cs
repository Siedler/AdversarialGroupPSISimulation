using System.Collections.Generic;

namespace Scrips.Agent {
	public class AgentEventHistoryManager {
		private List<string> _listOfEvents;

		private string _totalStringOfEvents;

		public AgentEventHistoryManager() {
			_listOfEvents = new List<string>();

			_totalStringOfEvents = "";
		}

		public void AddHistoryEvent(string historyEvent) {
			_listOfEvents.Add(historyEvent);
			_totalStringOfEvents += historyEvent + "\n\n";
		}

		public List<string> GetListOfEvents() {
			return _listOfEvents;
		}

		public string GetListOfEventsAsString() {
			return _totalStringOfEvents;
		}
	}
}