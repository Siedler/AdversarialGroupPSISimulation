using System.Collections.Generic;
using System.Linq;
using Scrips.Agent;
using Scrips.Agent.Memory;
using Scrips.Agent.Personality;
using UnityEngine;

public class CollectCloseFood : ActionPlanFoodRelated {
	private EnvironmentWorldCell _foodLocation;

	public CollectCloseFood(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
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

		_foodLocation = null;
		
		_eventHistoryManager.AddHistoryEvent("Collecting close food!");
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {

		// Current cell contains food -> Collect it
		if (currentEnvironmentWorldCell.ContainsFood()) return CollectFood(currentEnvironmentWorldCell);

		// Food was already eaten!
		if (_foodLocation != null && !_foodLocation.ContainsFood()) _foodLocation = null;

		// Search for food location
		if (_foodLocation == null) {
			_foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);
			if (_foodLocation == null) {
				OnFailure();
				return ActionResult.Failure;
			}
			
			_eventHistoryManager.AddHistoryEvent("Going to " + _foodLocation.cellCoordinates + " to collect the food!");
		}

		WalkTo(_foodLocation.cellCoordinates);
		return ActionResult.InProgress;
	}

	// This action plan is only executable, if no food cluster is in sight. Otherwise the food cluster action plan
	// is taken.
	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView) 
		       && agent.GetFoodCount() < SimulationSettings.MaximumStoredFoodCount
		       && !IsFoodClusterInSight(currentEnvironmentWorldCell, agentsFieldOfView);
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.CollectFoodOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.CollectFoodOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.CollectFoodOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.CollectFoodOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.CollectFoodOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.CollectFoodOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.CollectFoodOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.CollectFoodOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.CollectFoodOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.CollectFoodOnFailure[4];
	}
	
}