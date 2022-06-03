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
		AgentEventHistoryManager eventHistoryManager,
		Environment environment,
		Agent agentToHeal) : base(agent, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

		_agentToHeal = agentToHeal;
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0.5;
		expectedCertainty = 0.5;
		expectedCompetence = 0.5;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);
		
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
		
		OnSuccess();
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return nearbyAgents.Contains(_agentToHeal) && _agentToHeal.GetHealth() < 100;
	}
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0.5;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.3;
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return 0.5;
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return -0.1;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return -0.3;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.5;
	}
}
