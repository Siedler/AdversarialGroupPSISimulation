using System;
using System.Collections.Generic;
using System.Linq;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;
using Random = UnityEngine.Random;

public class Explore : ActionPlan {

	private Vector3Int _goalCoordinate;
	private bool _goalFound = false;

	public Explore(Agent agent, AgentPersonality agentPersonality, Hypothalamus hypothalamus, HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();
		
		_goalFound = false;
	}

	private bool AreThereUnexploredWorldCells() {
		Dictionary<Vector3Int, AgentMemoryWorldCell> agentWorldMemory = locationMemory.GetAgentsLocationMemory();
		
		foreach (AgentMemoryWorldCell agentMemoryWorldCell in agentWorldMemory.Values) {
			if(!agentMemoryWorldCell.IsExplored()) return true;
		}

		return false;
	}

	public override ActionResult Execute(
		EnvironmentWorldCell currentEnvironmentWorldCell,
		List<EnvironmentWorldCell> agentsFieldOfView,
		List<Agent> nearbyAgents) {
		
		if (!_goalFound) {
			List<AgentMemoryWorldCell> unexploredWorldCells = GetUnexploredWorldCells();

			// If there are no more unexplored world cells:
			// Take a random world cell weighted by their certainty score
			_goalCoordinate = GetNextCoordinateToExplore();
			_goalFound = true;

			_eventHistoryManager.AddHistoryEvent("I want to explore the cell with coordinates " + _goalCoordinate);
		}

		ActionResult actionResult = WalkTo(_goalCoordinate);

		if (actionResult == ActionResult.Success) {
			OnSuccess();
		} else if (actionResult == ActionResult.Failure) {
			OnFailure();
		}
		
		return actionResult;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return true;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.ExploreOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.ExploreOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.ExploreOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.ExploreOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.ExploreOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.ExploreOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.ExploreOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.ExploreOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.ExploreOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.ExploreOnFailure[4];
	}
}