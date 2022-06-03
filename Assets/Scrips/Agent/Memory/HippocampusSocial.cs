﻿
using System.Collections.Generic;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnityEngine;

public class HippocampusSocial {
	private Dictionary<Agent, AgentIndividualMemory> _agentIndividualMemory;

	// Saves the forget rate for the agents separated for positive and negative associations
	private double[] socialForgetRatePositiveNegative;

	public HippocampusSocial(AgentPersonality agentPersonality) {
		_agentIndividualMemory = new Dictionary<Agent, AgentIndividualMemory>();

		socialForgetRatePositiveNegative = new double[] {
			agentPersonality.GetValue("HippocampusSocialForgetRatePositive"),
			agentPersonality.GetValue("HippocampusSocialForgetRateNegative")
		};
	}

	public void SocialInfluence(Agent agent, double amount) {
		_agentIndividualMemory[agent].UpdateSocialScore(amount);
	}

	public void ReceiveSocialInfluence(Agent agent, double amount) {
		double newSocialScore = MathHelper.RunningAverage(
			_agentIndividualMemory[agent].GetSocialScore(), 
			amount,
			SimulationSettings.SocialMemoryReceiveNewKnownAgentAlphaFactor);
		
		_agentIndividualMemory[agent].SetSocialScore(newSocialScore);
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
}