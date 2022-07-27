using System;
using System.Collections.Generic;
using System.Text;

namespace Scrips.Agent {
	public class AgentEventHistoryManager {
		private string _name;

		private StringBuilder _stringBuilder;
		
		private List<string> _listOfEvents;

		public AgentEventHistoryManager(string name) {
			_name = name;
			
			_listOfEvents = new List<string>();
			_stringBuilder = new StringBuilder();
		}

		public void AddHistoryEvent(string historyEvent) {
			string stringToAdd = _name + ": " + historyEvent + "\n";
			
			//_listOfEvents.Add(stringToAdd);
			_stringBuilder.Append(stringToAdd);
		}

		public List<string> GetListOfEvents() {
			return _listOfEvents;
		}

		public string GetListOfEventsAsString() {
			return _stringBuilder.ToString();
		}

		public void Tick(int timeStep) {
			string stringToAdd = "\n" + "Time Step: " + timeStep + "\n"
			                     + "-----------------------" + "\n";
			//_listOfEvents.Add(stringToAdd);
			_stringBuilder.Append(stringToAdd);
		}
	}
}