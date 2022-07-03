using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class EatCloseFood : ActionPlanFoodRelated {
	
	private EnvironmentWorldCell _foodLocation;

	public EatCloseFood(
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
		
		_eventHistoryManager.AddHistoryEvent("Going to close food to eat it!");
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		
		// Current cell contains food -> Collect it
		if (currentEnvironmentWorldCell.ContainsFood()) return CollectAndEatFood(currentEnvironmentWorldCell);

		// Food was already eaten!
		if (_foodLocation != null && !_foodLocation.ContainsFood()) {
			_eventHistoryManager.AddHistoryEvent("Food was eaten! Go to a new food location.");
			_foodLocation = null;
		}

		// Search for food location
		if (_foodLocation == null) {
			_foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);

			if (_foodLocation == null) {
				OnFailure();
				return ActionResult.Failure;
			}
			
			_eventHistoryManager.AddHistoryEvent("Going to " + _foodLocation.cellCoordinates + " to eat the food!");
		}

		WalkTo(_foodLocation.cellCoordinates);
		return ActionResult.InProgress;
	}
	
	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView)
			&& !IsFoodClusterInSight(currentEnvironmentWorldCell, agentsFieldOfView);
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}
}