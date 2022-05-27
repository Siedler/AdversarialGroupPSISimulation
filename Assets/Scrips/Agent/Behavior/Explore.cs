
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class Explore : ActionPlan {

	private Vector3Int _goalCoordinate;
	private bool _goalFound = false;

	public Explore(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		Environment environment) : base(agent, hypothalamus, locationMemory, socialMemory, environment) {
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0;
		expectedCertainty = 0.5;
		expectedCompetence = 0.5;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		_goalFound = false;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		Dictionary<Vector3Int, AgentMemoryWorldCell> agentWorldMemory = locationMemory.GetAgentsLocationMemory();

		if (!_goalFound) {
			List<AgentMemoryWorldCell> unexploredWorldCells = new List<AgentMemoryWorldCell>();

			foreach (AgentMemoryWorldCell agentMemoryWorldCell in agentWorldMemory.Values) {
				if(agentMemoryWorldCell.IsExplored()) continue;
				
				unexploredWorldCells.Add(agentMemoryWorldCell);
			}

			int randomIndex = Random.Range(0, unexploredWorldCells.Count - 1);
			_goalCoordinate = unexploredWorldCells[randomIndex].cellCoordinates;
			_goalFound = true;
			
			Debug.Log(agent.name + ": I want to explore the cell with coordinates " + _goalCoordinate);
		}

		ActionResult actionResult = WalkTo(_goalCoordinate);

		if (actionResult == ActionResult.Success) {
			OnSuccess(0.0, 0.0, 0.0, 0.5, 0.5);
		} else if (actionResult == ActionResult.Failure) {
			OnFailure();
		}
		
		return actionResult;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return true;
	}
}