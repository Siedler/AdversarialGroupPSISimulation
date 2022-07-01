using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class GiveFood : ActionPlan {
	private readonly Agent _correspondingAgent;
	
	private bool _wasRequested;
	
	public GiveFood(Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment,
		Agent correspondingAgent) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory,
		eventHistoryManager, environment) {

		_correspondingAgent = correspondingAgent;
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake = GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	private void GiveFoodToAgent() {
		agent.ConsumeFoodFromStorage();
		_correspondingAgent.ReceiveFood(agent);
		
		socialMemory.SocialInfluence(_correspondingAgent, 0.1);
	}
	
	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		int agentInRange = IsAgentInRange(agentsFieldOfView, _correspondingAgent);

		if (agentInRange == -1) {
			EnvironmentWorldCell environmentWorldCellOfAgentToExchangeInformation =
				IsAgentInFieldOfView(agentsFieldOfView, _correspondingAgent);

			if (environmentWorldCellOfAgentToExchangeInformation == null) {
				OnFailure();
				return ActionResult.Failure;
			}

			WalkTo(environmentWorldCellOfAgentToExchangeInformation.cellCoordinates);

			// Recheck if agent is now in range. If so -> give information
			agentInRange = IsAgentInRange(agentsFieldOfView, _correspondingAgent);
			if (agentInRange != -1) {
				GiveFoodToAgent();
				OnSuccess();
				return ActionResult.Success;
			}

			return ActionResult.InProgress;
		}

		// Agent is in range -> Exchange Social Information
		GiveFoodToAgent();
		OnSuccess();
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		_wasRequested = _wasRequested && nearbyAgents.Contains(_correspondingAgent) && agent.HasFood();
		return nearbyAgents.Contains(_correspondingAgent) && agent.HasFood();
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return (_wasRequested = (_wasRequested && nearbyAgents.Contains(_correspondingAgent) && agent.HasFood())) ? 0.1 : 0;
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.GiveFoodOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.GiveFoodOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		// the friendlyModifier adds an incentive to help friends more often than non-friends
		double friendlyModifier = socialMemory.GetSocialScore(_correspondingAgent) + 1;
		return SimulationSettings.GiveFoodOnSuccess[2] * friendlyModifier;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.GiveFoodOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.GiveFoodOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.GiveFoodOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.GiveFoodOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		// the friendlyModifier adds an incentive to help friends more often than non-friends
		double friendlyModifier = socialMemory.GetSocialScore(_correspondingAgent) + 1;
		return SimulationSettings.GiveFoodOnFailure[2] * friendlyModifier;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.GiveFoodOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.GiveFoodOnFailure[4]; 
	}

	public void RegisterRequest() {
		_wasRequested = true;
	}
}