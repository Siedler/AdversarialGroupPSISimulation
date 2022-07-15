using System.Collections.Generic;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnityEngine;

public class HippocampusSocial {
	private Dictionary<Agent, AgentIndividualMemory> _agentIndividualMemory;

	// Save the team association
	// Important for export
	private int _team;
	
	// Saves the forget rate for the agents separated for positive and negative associations
	private double[] socialForgetRatePositiveNegative;

	public HippocampusSocial(AgentPersonality agentPersonality, int team) {
		_agentIndividualMemory = new Dictionary<Agent, AgentIndividualMemory>();

		_team = team;
		
		socialForgetRatePositiveNegative = new double[] {
			agentPersonality.GetValue("HippocampusSocialForgetRatePositive"),
			agentPersonality.GetValue("HippocampusSocialForgetRateNegative")
		};
	}

	public void SocialInfluence(Agent agent, double amount) {
		_agentIndividualMemory[agent].UpdateSocialScore(amount);
	}

	// Updating the social score of another agent based on the opinion of the other agent of the agent in question (amount)
	// and tacking it into account by the a value of socialMemoryReceiveNewKnownAgentAlphaFactor
	// new score = (socialMemoryReceiveNewKnownAgentAlphaFactor * amount) + (1 - socialMemoryReceiveNewKnownAgentAlphaFactor) * _agentIndividualMemory[agent].GetSocialScore()
	public void ReceiveSocialInfluence(Agent agent, double amount, double socialMemoryReceiveNewKnownAgentAlphaFactor) {
		double newSocialScore = MathHelper.RunningAverage(
			_agentIndividualMemory[agent].GetSocialScore(), 
			amount,
			socialMemoryReceiveNewKnownAgentAlphaFactor);
		
		_agentIndividualMemory[agent].SetSocialScore(newSocialScore);
	}

	public double GetSocialScore(Agent agent) {
		return _agentIndividualMemory[agent].GetSocialScore();
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

	public Dictionary<Agent, AgentIndividualMemory> GetAgentIndividualMemory() {
		return _agentIndividualMemory;
	}

	public (Agent, AgentIndividualMemory) GetRandomAgentMemory(Agent agentToExclude = null) {
		List<Agent> allAgents = new List<Agent>(_agentIndividualMemory.Keys);
		
		if (agentToExclude != null && allAgents.Contains(agentToExclude)) {
			allAgents.Remove(agentToExclude);
		}

		int randomAgent = Random.Range(0, allAgents.Count - 1);

		return (allAgents[randomAgent], _agentIndividualMemory[allAgents[randomAgent]]);
	}

	public void Tick() {
		foreach (AgentIndividualMemory agentIndividualMemory in _agentIndividualMemory.Values) {
			agentIndividualMemory.Forget(socialForgetRatePositiveNegative);
		}	
	}

	public string ToJson() {
		string jsonString = "[\n";
		int i = 0;
		foreach ((Agent agent, AgentIndividualMemory agentIndividualMemory) in _agentIndividualMemory) {
			string sameTeam = agent.GetTeam() == _team ? "true" : "false";
			jsonString +=
				"{\"name\" : \"" + agent.name + "\"" 
				+ ", \"social_score\" : " + agentIndividualMemory.GetSocialScore()
				+ ", \"same_team\" : " + sameTeam + "}";
			jsonString += i != _agentIndividualMemory.Count - 1 ? ",\n" : "\n";
			i++;
		}
		jsonString += "]";
		
		return jsonString;
	}
}