
using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;

public class RequestHealing : ActionPlan {

	private bool _calledOutForHealing;

	private int _timeRemainingToWait;
	private double _initialHealth;
	
	public RequestHealing(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality,
		hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {
		
		expectedPainAvoidance = 0.2;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0.3;
		expectedCertainty = 0.3;
		expectedCompetence = 0.2;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);
		_calledOutForHealing = false;
		_timeRemainingToWait = 7;

		_initialHealth = agent.GetHealth();
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		if (!_calledOutForHealing) {
			CallOutRequest(RequestType.Healing, agentsFieldOfView, null);
			_calledOutForHealing = true;
			return ActionResult.InProgress;
		}
		
		// Wait
		if (agent.GetHealth() > _initialHealth) {
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
		return nearbyAgents.Count > 0 && agent.GetHealth() < 100;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		// Urgency is higher if the health is lower
		return 0.1 * (1 - (agent.GetHealth() / 100.0));
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0.2;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0.3;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.3;
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
		return -0.3;
	}
}