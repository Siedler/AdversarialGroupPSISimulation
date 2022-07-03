using System;
using System.Collections.Generic;
using System.Linq;
using Scrips.Agent;
using Scrips.Agent.Memory;
using Scrips.Agent.Personality;

public abstract class ActionPlanFoodRelated : ActionPlan{
	protected ActionPlanFoodRelated(
		Agent agent,
		AgentPersonality agentPersonality,
	Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) { }
	
	protected ActionResult CollectAndEatFood(EnvironmentWorldCell currentEnvironmentWorldCell) {
		if (!currentEnvironmentWorldCell.ContainsFood()) throw new InvalidOperationException("Tried to collect food even though the world cell has no food");

		_eventHistoryManager.AddHistoryEvent("Eating food from " + currentEnvironmentWorldCell.cellCoordinates);
		
		currentEnvironmentWorldCell.ConsumeFood();

		OnSuccess();
		return ActionResult.Success;
	}

	protected ActionResult CollectFood(EnvironmentWorldCell currentEnvironmentWorldCell) {
		if (!currentEnvironmentWorldCell.ContainsFood()) throw new InvalidOperationException("Tried to collect food even though the world cell has no food");

		_eventHistoryManager.AddHistoryEvent("Collecting food from " + currentEnvironmentWorldCell.cellCoordinates);
		
		agent.CollectFood();
		currentEnvironmentWorldCell.ConsumeFood();
		
		OnSuccess();
		return ActionResult.Success;
	}

	protected bool IsFoodInRange(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView) {
		if (currentEnvironmentWorldCell.ContainsFood()) return true;
		
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if(environmentWorldCell == null) continue;
			if (environmentWorldCell.ContainsFood()) return true;
		}

		return false;
	}
	
	// Check if any food cluster is insight the field of view
	protected bool IsFoodClusterInSight(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView) {
		IEnumerable <FoodCluster> foodClusters = agent.GetFoodClusters();

		// Add the current world cell to the list of agentsFieldOfView to make calculation easier
		agentsFieldOfView.Add(currentEnvironmentWorldCell);
		IEnumerable<EnvironmentWorldCell> worldCellsInRange = agentsFieldOfView.Where(x => x != null);
		
		foreach(FoodCluster cluster in foodClusters) {
			if (worldCellsInRange.Any(x => x.cellCoordinates == cluster.GetCenter().cellCoordinates))
				return true;
		}

		return false;
	}
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.FoodRelatedOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.FoodRelatedOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.FoodRelatedOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.FoodRelatedOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.FoodRelatedOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.FoodRelatedOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.FoodRelatedOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.FoodRelatedOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.FoodRelatedOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.FoodRelatedOnFailure[4];
	}
}