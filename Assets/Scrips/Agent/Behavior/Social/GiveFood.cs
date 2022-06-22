
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
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0.3;
		expectedCertainty = 0.2;
		expectedCompetence = 0.3;
	}

	private void GiveFoodToAgent() {
		agent.ConsumeFoodFromStorage();
		_correspondingAgent.ReceiveFood(agent);
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
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0.3;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.2;
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return 0.3;
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

	public void RegisterRequest() {
		_wasRequested = true;
	}
}