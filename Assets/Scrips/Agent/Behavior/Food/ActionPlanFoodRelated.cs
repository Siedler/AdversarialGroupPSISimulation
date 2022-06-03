
using System;
using Scrips.Agent;

public abstract class ActionPlanFoodRelated : ActionPlan{
	protected ActionPlanFoodRelated(
		Agent agent,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) { }

	protected ActionResult CollectAndEatFood(EnvironmentWorldCell currentEnvironmentWorldCell) {
		if (!currentEnvironmentWorldCell.ContainsFood()) throw new InvalidOperationException("Tried to collect food even though the world cell has no food");

		currentEnvironmentWorldCell.ConsumeFood();

		OnSuccess();
		return ActionResult.Success;
	}
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0.5;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.4;
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
		return -0.3;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.5;
	}
}