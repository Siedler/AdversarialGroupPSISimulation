

using System.Collections.Generic;
using UnityEngine;

public class SelfHeal : ActionPlan {
	public SelfHeal(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		Environment environment) : base(agent, hypothalamus, locationMemory, socialMemory, environment) {
		expectedPainAvoidance = 0.5;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0;
		expectedCertainty = 0.3;
		expectedCompetence = 0.5;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) { }

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		agent.Heal(10);
		
		OnSuccess(0.1, 0, 0, 0, 0.5);
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return agent.GetHealth() < 100;
	}
}