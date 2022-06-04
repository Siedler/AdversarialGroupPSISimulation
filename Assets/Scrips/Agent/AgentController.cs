using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Scrips.Agent;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentController : MonoBehaviour {

	public int numberOfAgents = 1;
	private int _teamNumber;
	private Direction _spawnDirection;
	
	private Environment _environment;

	private EnvironmentWorldCell[] _spawnPoints;
	
	public GameObject agentPrefab;
	private List<GameObject> _agents;

	private Queue<GameObject> _agentsToRespawn;

	public void InitiateAgents(EnvironmentWorldCell[] spawnPoints, int teamNumber, Direction spawnDirection = Direction.E) {
		_environment = GameObject.Find("World").GetComponent<Environment>();
		_agents = new List<GameObject>();

		_spawnPoints = spawnPoints;

		this._teamNumber = teamNumber;
		this._spawnDirection = spawnDirection;

		this._agentsToRespawn = new Queue<GameObject>();
		
		// Spawn all agents but set them as not active
		for (int i = 0; i < numberOfAgents; i++) {
			GameObject newAgentObject =
				GameObject.Instantiate(agentPrefab, Vector3.zero, Quaternion.identity, this.transform);
			newAgentObject.SetActive(false);
			Agent newAgent = newAgentObject.GetComponent<Agent>();
			
			AgentPersonality _agentPersonality = GenerateAgentPersonality();
			
			newAgent.InitiateAgent(teamNumber, _agentPersonality, this);
			
			_agents.Add(newAgentObject);
			_agentsToRespawn.Enqueue(newAgentObject);
		}
	}

	// Execute actions of one time-step
	public void Tick() {
		SpawnAllQueuedAgentsIfPossible();

		foreach (GameObject agent in _agents) {
			agent.GetComponent<Agent>().Tick();
		}
	}

	// Generate the per
	private AgentPersonality GenerateAgentPersonality() {
		// Set all values for the agent personality!
		AgentPersonality _agentPersonality = new AgentPersonality();
		
		// TODO Introduce some variance
		
		// Set the leakage values for the hypothalamus
		_agentPersonality.SetValue("HypothalamusPainAvoidanceLeakage", 0);
		_agentPersonality.SetValue("HypothalamusEnergyLeakage",
			MathHelper.NextGaussian(
				SimulationSettings.LeakageValueEnergyMean,
				SimulationSettings.LeakageValueEnergySigma, 
				0, 
				1));
		_agentPersonality.SetValue("HypothalamusAffiliationLeakage",
			MathHelper.NextGaussian(
				SimulationSettings.LeakageValueAffiliationMean,
				SimulationSettings.LeakageValueAffiliationSigma, 
				0, 
				1));
		_agentPersonality.SetValue("HypothalamusCertaintyLeakage",MathHelper.NextGaussian(
			SimulationSettings.LeakageValueCertaintyMean,
			SimulationSettings.LeakageValueAffiliationSigma, 
			0, 
			1));
		_agentPersonality.SetValue("HypothalamusCompetenceLeakage",MathHelper.NextGaussian(
			SimulationSettings.LeakageValueCompetenceMean,
			SimulationSettings.LeakageValueCompetenceSigma, 
			0, 
			1));
		
		// Set the positive and negative forget rate for the location memory of the agent
		_agentPersonality.SetValue("HippocampusLocationPainAvoidanceForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusLocationPainAvoidanceForgetRateNegative", 0.9);
		
		_agentPersonality.SetValue("HippocampusLocationEnergyForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusLocationEnergyForgetRateNegative", 0.9);
		
		_agentPersonality.SetValue("HippocampusLocationAffiliationForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusLocationAffiliationForgetRateNegative", 0.9);
		
		_agentPersonality.SetValue("HippocampusLocationCertaintyForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusLocationCertaintyForgetRateNegative", 0.9);
		
		_agentPersonality.SetValue("HippocampusLocationCompetenceForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusLocationCompetenceForgetRateNegative", 0.9);
		
		// Set the positive and negative forget rate for social memory of the agent
		_agentPersonality.SetValue("HippocampusSocialForgetRatePositive", 0.95);
		_agentPersonality.SetValue("HippocampusSocialForgetRateNegative", 0.9);
		
		// Set values for receiving agent information
		// A factor of how much the agent takes in the information about the previously unknown agent
		// Situation: An agent receives social information about another agent he/she does not know yet. The value of how
		// much the agent is influenced by the previous agent received information is controlled by this factor.
		// 0 < x <= 1
		_agentPersonality.SetValue("SocialMemoryReceiveNewUnknownAgentSoftenFactor", 0.8);
		// A factor regulating how much the newly shared information about a known agent is taken into account.
		// Example: I have a friend that I like by 0.8. Another friends tells me they like them only 0.5. I'll get influenced
		// by a small factor by this saying. This factor regulated this here. So the new value would be:
		// (0.5 * SocialMemoryReceiveNewKnownAgentAlphaFactor) + (0.8 * (1 - SocialMemoryReceiveNewKnownAgentAlphaFactor))
		// 0 <= x <= 1
		_agentPersonality.SetValue("SocialMemoryReceiveNewKnownAgentAlphaFactor", 0.3);

		// Selection bias value, i.e. a value that is subtracted from every motive that is currently not being followed
		// TODO how to use this value? Just subtract it form every other motive?
		_agentPersonality.SetValue("SelectionThreshold", 0.02);
		
		return _agentPersonality;
	}

	public void RegisterToRespawn(GameObject agentObject) {
		_agentsToRespawn.Enqueue(agentObject);
	}
	
	private List<EnvironmentWorldCell> GetEmptySpawnLocations() {
		List<EnvironmentWorldCell> emptyWorldCells = new List<EnvironmentWorldCell>();

		foreach (EnvironmentWorldCell worldCell in _spawnPoints) {
			if(!worldCell.IsOccupied()) emptyWorldCells.Add(worldCell);
		}

		return emptyWorldCells;
	}

	private void SpawnAllQueuedAgentsIfPossible() {
		while (_agentsToRespawn.Count > 0) {
			GameObject agentToSpawn = _agentsToRespawn.Peek();
			
			// Try to spawn Agent. If not possible => break out of the array
			if(!SpawnAgentAtRandomPosition(agentToSpawn)) break;
			
			_agentsToRespawn.Dequeue();
		}
	}
	
	private bool SpawnAgentAtRandomPosition(GameObject agentObject) {
		Agent agent = agentObject.GetComponent<Agent>();
		
		List<EnvironmentWorldCell> emptySpawnLocations = GetEmptySpawnLocations();

		// If no empty spawn points are available return false, i.e. failure to spawn
		if (emptySpawnLocations.Count == 0) return false;
		
		int randomEmptyTile = Random.Range(0, emptySpawnLocations.Count-1);
		EnvironmentWorldCell environmentWorldCell = emptySpawnLocations[randomEmptyTile];
		
		agent.Spawn(environmentWorldCell, _spawnDirection);
		
		return true;
	}
}
