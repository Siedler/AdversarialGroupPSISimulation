using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Scrips.Agent;
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
			newAgent.InitiateAgent(teamNumber, this);
			
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
