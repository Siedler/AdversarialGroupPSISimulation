using System.Collections.Generic;

namespace Scrips.Agent {
	public class AgentEventHistoryManager {
		private string _name;
		private List<string> _listOfEvents;

		private string _totalStringOfEvents;

		public AgentEventHistoryManager(string name) {
			_name = name;
			
			_listOfEvents = new List<string>();

			_totalStringOfEvents = "";
		}

		public void AddHistoryEvent(string historyEvent) {
			string stringToAdd = _name + ": " + historyEvent + "\n";
			
			_listOfEvents.Add(stringToAdd);
			_totalStringOfEvents += stringToAdd;
		}

		public List<string> GetListOfEvents() {
			return _listOfEvents;
		}

		public string GetListOfEventsAsString() {
			return _totalStringOfEvents;
		}

		public void Tick(int timeStep) {
			string stringToAdd = "\n" + "Time Step: " + timeStep + "\n"
			                     + "-----------------------" + "\n";
			_listOfEvents.Add(stringToAdd);
			_totalStringOfEvents += stringToAdd;
		}
	}
}