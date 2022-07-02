using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

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
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();

		_calledOutForFood = false;
		_timeRemainingToWait = 5;
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
		return SimulationSettings.CallForFoodOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.CallForFoodOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.CallForFoodOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.CallForFoodOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.CallForFoodOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.CallForFoodOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.CallForFoodOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.CallForFoodOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.CallForFoodOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.CallForFoodOnFailure[4];
	}
}
