	
using System.Collections.Generic;
using UnityEngine;

public class EatFood : ActionPlanFoodRelated{

	public EatFood(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		Environment environment) : base(agent, hypothalamus, locationMemory, socialMemory, environment) {
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0.5;
		expectedAffiliation = 0;
		expectedCertainty = 0;
		expectedCompetence = 0.5;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		if (!agent.HasFood()) {
			OnFailure();
			return ActionResult.Failure;
		}

		agent.EatFoodFromStorage();
		
		OnSuccess();
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return agent.HasFood();
	}
}