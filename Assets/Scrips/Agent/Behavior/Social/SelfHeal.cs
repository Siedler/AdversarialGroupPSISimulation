using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class SelfHeal : ActionPlan {
	public SelfHeal(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {
		expectedPainAvoidance = 0.5;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0;
		expectedCertainty = 0.3;
		expectedCompetence = 0.5;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		agent.Heal(10);
		
		OnSuccess();
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return agent.GetHealth() < 100;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 4*(1-hypothalamus.GetPainAvoidanceDifference());
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return -0.2;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0;
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
		return 0;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return 0;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.3;
	}
}