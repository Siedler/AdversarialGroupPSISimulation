using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

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
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();
		_calledOutForHealing = false;
		_timeRemainingToWait = 5;

		_initialHealth = agent.GetHealth();
		
		_eventHistoryManager.AddHistoryEvent("Started ActionPlan to request healing");
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
		return SimulationSettings.RequestHealingOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.RequestHealingOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.RequestHealingOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.RequestHealingOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.RequestHealingOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.RequestHealingOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.RequestHealingOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.RequestHealingOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.RequestHealingOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.RequestHealingOnFailure[4];
	}
}