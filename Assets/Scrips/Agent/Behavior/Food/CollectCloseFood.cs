using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class CollectCloseFood : ActionPlanFoodRelated {
	private EnvironmentWorldCell _foodLocation;

	public CollectCloseFood(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);
		
		_foodLocation = null;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {

		// Current cell contains food -> Collect it
		if (currentEnvironmentWorldCell.ContainsFood()) return CollectFood(currentEnvironmentWorldCell);

		// Food was already eaten!
		if (_foodLocation != null && !_foodLocation.ContainsFood()) _foodLocation = null;

		// Search for food location
		if (_foodLocation == null) {
			_foodLocation = GetClosestFoodLocationInFieldOfView(agentsFieldOfView);
			if (_foodLocation == null) {
				OnFailure();
				return ActionResult.Failure;
			}
		}

		WalkTo(_foodLocation.cellCoordinates);
		return ActionResult.InProgress;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return IsFoodInRange(currentEnvironmentWorldCell, agentsFieldOfView) && agent.GetFoodCount() < SimulationSettings.MaximumStoredFoodCount;;
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return 0;
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.6;
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