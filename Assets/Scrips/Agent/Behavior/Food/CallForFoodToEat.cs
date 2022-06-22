
using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;

public class CallForFoodToEat : ActionPlanFoodRelated {

	private bool _calledOutForFood;
	private int _timeRemainingToWait;
	private int _prevFoodCount;
	
	public CallForFoodToEat(Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory,
		eventHistoryManager, environment) {
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0.5;
		expectedAffiliation = 0.2;
		expectedCertainty = 0.2;
		expectedCompetence = 0.2;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);

		_calledOutForFood = false;
		_timeRemainingToWait = 7;
		_prevFoodCount = agent.GetFoodCount();
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		if (!_calledOutForFood) {
			CallOutRequest(RequestType.Food, agentsFieldOfView, null);
			_calledOutForFood = true;
			return ActionResult.InProgress;
		}
		
		// Wait
		if (agent.GetFoodCount() > _prevFoodCount) {
			agent.ConsumeFoodFromStorage();
			OnSuccess();
			return ActionResult.Success;
		}

		_timeRemainingToWait--;
		if (_timeRemainingToWait == 0) {
			OnFailure();
			return ActionResult.Failure;
		}

		return ActionResult.InProgress;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return nearbyAgents.Count > 0;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		// The higher the energy difference is the more urgent is this behaviour
		return 0.1 * (1 - (hypothalamus.GetCurrentEnergyValue()));
	}
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0.5;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0.2;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.2;
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return 0.2;
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return -0.2;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return -0.2;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.2;
	}
}
