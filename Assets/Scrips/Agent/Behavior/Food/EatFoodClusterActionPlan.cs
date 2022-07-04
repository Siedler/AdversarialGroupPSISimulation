using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Memory;
using Scrips.Agent.Personality;

public class EatFoodClusterActionPlan : ActionPlanFoodRelated {
	private FoodCluster _foodCluster;

	private SearchForFoodToEat _searchForFoodToEat;
	
	private EnvironmentWorldCell _foodLocation;

	private bool _reachedFoodCluster;

	public EatFoodClusterActionPlan(Agent agent, AgentPersonality agentPersonality, Hypothalamus hypothalamus,
		HippocampusLocation locationMemory, HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager, Environment environment, FoodCluster correspondingFoodCluster, SearchForFoodToEat searchForFoodToEat) :
		base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

		_foodCluster = correspondingFoodCluster;

		_searchForFoodToEat = searchForFoodToEat;
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();

		_foodLocation = null;
		_reachedFoodCluster = false;
		
		_eventHistoryManager.AddHistoryEvent("Walking to food cluster with coordinates " + _foodCluster.GetCenter() + " to eat food!");
	}

	private bool IsFoodClusterCenterInFieldOfView(List<EnvironmentWorldCell> agentsFieldOfView) {
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if(environmentWorldCell == null) continue;
			
			if (environmentWorldCell.cellCoordinates == _foodCluster.GetCenter().cellCoordinates) return true;
		}

		return false;
	}
	
	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		if (IsFoodClusterCenterInFieldOfView(agentsFieldOfView)) _reachedFoodCluster = true;

		if (_reachedFoodCluster) {
			if (!IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView)) {
				OnFailure();
				return ActionResult.Failure;
			}
			
			// If the current environmentWorldCell contains food => Eat it!
			if (currentEnvironmentWorldCell.ContainsFood()) return CollectAndEatFood(currentEnvironmentWorldCell);
			
			// Food was already eaten!
			if (_foodLocation != null && !_foodLocation.ContainsFood()) {
				_eventHistoryManager.AddHistoryEvent("Food was already eaten! Search for a new food location.");
				_foodLocation = null;
			}

			// Search for food location
			if (_foodLocation == null) {
				_foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);
				
				_eventHistoryManager.AddHistoryEvent("Going to " + _foodLocation.cellCoordinates + " to eat food!");
				
				if (_foodLocation == null) {
					OnFailure();
					return ActionResult.Failure;
				}
			}

			WalkTo(_foodLocation.cellCoordinates);
			return ActionResult.InProgress;
		}

		WalkTo(_foodCluster.GetCenter().cellCoordinates);
		
		return ActionResult.InProgress;
	}

	protected override void OnFailure() {
		base.OnFailure();

		_searchForFoodToEat.Activate();
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return true;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return IsFoodClusterCenterInFieldOfView(agentsFieldOfView) ? 0.1 : 0;
	}
}