using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;

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
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0.5;
		expectedAffiliation = 0;
		expectedCertainty = 0;
		expectedCompetence = 0.5;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);
		
		_foodLocation = null;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		
		// Current cell contains food -> Collect it
		if (currentEnvironmentWorldCell.ContainsFood()) return CollectAndEatFood(currentEnvironmentWorldCell);

		// Food was already eaten!
		if (_foodLocation != null && !_foodLocation.ContainsFood()) _foodLocation = null;

		// Search for food location
		if (_foodLocation == null) {
			_foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);
			if (_foodLocation == null) {
				OnFailure();
				return ActionResult.Failure;
			}
		}

		WalkTo(_foodLocation.cellCoordinates);
		return ActionResult.InProgress;
	}
	
	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		if (currentEnvironmentWorldCell.ContainsFood()) return true;
		
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if(environmentWorldCell == null) continue;
			if (environmentWorldCell.ContainsFood()) return true;
		}

		return false;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}
}