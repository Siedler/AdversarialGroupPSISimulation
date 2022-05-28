
using System.Collections.Generic;

public class HippocampusSocial {
	private Dictionary<Agent, AgentIndividualMemory> _agentIndividualMemory;

	public HippocampusSocial() {
		_agentIndividualMemory = new Dictionary<Agent, AgentIndividualMemory>();
	}

	public void SocialInfluence(Agent agent, double amount) {
		_agentIndividualMemory[agent].UpdateSocialScore(amount);
	}

	public bool KnowsAgent(Agent agent) {
		return _agentIndividualMemory.ContainsKey(agent);
	}

	public void AddNewlyMetAgent(Agent agent, double initialSocialScore) {
		_agentIndividualMemory.Add(agent, new AgentIndividualMemory(initialSocialScore));
	}
	
	public AgentIndividualMemory GetIndividualMemory(Agent agent) {
		return _agentIndividualMemory[agent];
	}
	
	public void Tick() {
		foreach (AgentIndividualMemory agentIndividualMemory in _agentIndividualMemory.Values) {
			agentIndividualMemory.Tick();
		}	
	}
}