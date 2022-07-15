using System.Collections.Generic;
using System.Linq;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;

public class SearchForFoodToEat : ActionPlanFoodRelated{
	
	private Vector3Int _goalCoordinate;

	// States:
	// 0 = start
	// 1 = explore
	// 2 = found food!
	private int _state = 0;

	private bool _activated;

	public SearchForFoodToEat(Agent agent, AgentPersonality agentPersonality, Hypothalamus hypothalamus,
		HippocampusLocation locationMemory, HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager, Environment environment) : base(agent, agentPersonality,
		hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

		_activated = false;
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();

		_state = 0;
		_eventHistoryManager.AddHistoryEvent("Started search for food!");
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {

		if (currentEnvironmentWorldCell.ContainsFood()) {
			// Food reached! Eat it
			CollectAndEatFood(currentEnvironmentWorldCell);
			OnSuccess();
			return ActionResult.Success;
		}

		if (_state == 0) {
			_goalCoordinate = GetNextCoordinateToExplore();
			_state = 1;
			_eventHistoryManager.AddHistoryEvent("Next coordinate to explore for food: " + _goalCoordinate);

			WalkTo(_goalCoordinate);
			return ActionResult.InProgress;
		}

		if (_state == 1) {
			if (IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView)) {
				// Food Found! Go to food and eat it!

				EnvironmentWorldCell foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);
				
				// Failed to get closest food location in range
				if (foodLocation == null) {
					OnFailure();
					return ActionResult.Failure;
				}

				_state = 2;
				_goalCoordinate = foodLocation.cellCoordinates;
				WalkTo(_goalCoordinate);
				
				_eventHistoryManager.AddHistoryEvent("Found food in range at: " + _goalCoordinate + "!");

				return ActionResult.InProgress;
			}

			if (currentEnvironmentWorldCell.cellCoordinates == _goalCoordinate) {
				// reached goal coordinate => search new goal coordinate
				agent.Experience(
					SimulationSettings.SearchForFoodIntermediateExploreReward[0],
					SimulationSettings.SearchForFoodIntermediateExploreReward[1],
					SimulationSettings.SearchForFoodIntermediateExploreReward[2],
					SimulationSettings.SearchForFoodIntermediateExploreReward[3],
					SimulationSettings.SearchForFoodIntermediateExploreReward[4]
				);
				
				_goalCoordinate = GetNextCoordinateToExplore();
				WalkTo(_goalCoordinate);
				_eventHistoryManager.AddHistoryEvent(
					"No found found on exploring of last tile found! Searching now at: " + _goalCoordinate);
				return ActionResult.InProgress;
			}

			WalkTo(_goalCoordinate);
			return ActionResult.InProgress;
		}

		// _state == 2
		// Food found!
		if (!IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView)) {
			_goalCoordinate = GetNextCoordinateToExplore();
			_state = 1;
			_eventHistoryManager.AddHistoryEvent("Food disappeared on my way there! Searching for new food location at: " + _goalCoordinate);

			WalkTo(_goalCoordinate);
			return ActionResult.InProgress;
		}

		WalkTo(_goalCoordinate);
		return ActionResult.InProgress;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return (_activated && !IsFoodClusterInSight(currentEnvironmentWorldCell, agentsFieldOfView)) || !agent.GetFoodClusters().Any();
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}
	
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.SearchForFoodOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.SearchForFoodOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.SearchForFoodOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.SearchForFoodOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.SearchForFoodOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.SearchForFoodOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.SearchForFoodOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.SearchForFoodOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.SearchForFoodOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.SearchForFoodOnFailure[4];
	}

	protected override void OnSuccess() {
		base.OnSuccess();
		
		_activated = false;
	}

	protected override void OnFailure() {
		base.OnFailure();

		_activated = false;
	}

	public void Activate() {
		_activated = true;
	}
	
}