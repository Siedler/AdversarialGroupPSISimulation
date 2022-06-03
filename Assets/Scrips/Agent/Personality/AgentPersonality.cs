using Scrips.DataStructures.SerializableDictionary;

namespace Scrips.Agent.Personality {
	public class AgentPersonality {
		private DictionaryStringDouble _agentPersonality = new DictionaryStringDouble();

		public double GetValue(string key) {
			return _agentPersonality[key];
		}

		public void SetValue(string key, double value) {
			if (_agentPersonality.ContainsKey(key)) {
				_agentPersonality[key] = value;
				return;
			}
			
			_agentPersonality.Add(key, value);
		}
	}
}