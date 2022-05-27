using System.Collections.Generic;
using Scrips.Agent;
using UnityEngine;

public class GoHeal : ActionPlan {

	private Agent _agentToHeal;
	
	public GoHeal(
		Agent agent,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		Environment environment,
		Agent agentToHeal) : base(agent, hypothalamus, locationMemory, socialMemory, environment) {

		_agentToHeal = agentToHeal;
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0.5;
		expectedCertainty = 0.5;
		expectedCompetence = 0.5;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		_agentToHeal = correspondingAgent;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {

		int agentInRange = IsAgentInRange(agentsFieldOfView, _agentToHeal);

		if (agentInRange == -1) {
			EnvironmentWorldCell environmentWorldCellOfAgentToHeal =
				IsAgentInFieldOfView(agentsFieldOfView, _agentToHeal);

			if (environmentWorldCellOfAgentToHeal == null) {
				OnFailure();
				return ActionResult.Failure;
			}
			
			WalkTo(environmentWorldCellOfAgentToHeal.cellCoordinates);
			return ActionResult.InProgress;
		}

		_agentToHeal.Heal(10, _agentToHeal);
		agent.SetOrientation((Direction) agentInRange);
		
		OnSuccess(0, 0, 0.3, 0, 0.5);
		return ActionResult.Success;
	}

	// TODO check if there are any hurt agents
	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return nearbyAgents.Contains(_agentToHeal) && _agentToHeal.GetHealth() < 100;
	}
}
