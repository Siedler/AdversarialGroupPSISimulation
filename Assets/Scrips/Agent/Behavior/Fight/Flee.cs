using System.Collections.Generic;
using Priority_Queue;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class Flee : ActionPlan {

	private Agent _agentToFleeFrom;
	private AgentMemoryWorldCell _worldCellAgentFeelsMostCertain;

	public Flee(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment,
		Agent agentToFleeFrom) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory,
		eventHistoryManager, environment) {
		
		_agentToFleeFrom = agentToFleeFrom;
	}

	private AgentMemoryWorldCell GetAgentMemoryWorldCellToFleeTo(EnvironmentWorldCell currentEnvironmentWorldCell) {
		SimplePriorityQueue<AgentMemoryWorldCell> agentMemoryWorldCells =
			new SimplePriorityQueue<AgentMemoryWorldCell>();

		foreach (AgentMemoryWorldCell agentMemoryWorldCell in locationMemory.GetAgentsLocationMemory().Values) {
			float preferenceFleeScore = (float) agentMemoryWorldCell.GetNeedSatisfactionAssociations()[3];
				
			agentMemoryWorldCells.Enqueue(agentMemoryWorldCell, -preferenceFleeScore);
		}

		// Get the best world cell to flee to
		AgentMemoryWorldCell worldCellAgentFeelsMostCertain = agentMemoryWorldCells.Dequeue();
		if (worldCellAgentFeelsMostCertain.cellCoordinates == currentEnvironmentWorldCell.cellCoordinates) {
			worldCellAgentFeelsMostCertain = agentMemoryWorldCells.Dequeue();
		}

		return worldCellAgentFeelsMostCertain;
	}
	
	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<global::Agent> nearbyAgents) {
		if (!nearbyAgents.Contains(_agentToFleeFrom)) {
			OnSuccess();
			return ActionResult.Success;
		}

		if (_worldCellAgentFeelsMostCertain == null) {
			_worldCellAgentFeelsMostCertain = GetAgentMemoryWorldCellToFleeTo(currentEnvironmentWorldCell);

			WalkTo(_worldCellAgentFeelsMostCertain.cellCoordinates);
			return ActionResult.InProgress;
		}

		// Agent has arrived at destination to flee to but the other agent is still in range
		if (currentEnvironmentWorldCell.cellCoordinates == _worldCellAgentFeelsMostCertain.cellCoordinates) {
			_worldCellAgentFeelsMostCertain = GetAgentMemoryWorldCellToFleeTo(currentEnvironmentWorldCell);
			return ActionResult.InProgress;
		}

		WalkTo(_worldCellAgentFeelsMostCertain.cellCoordinates);
		return ActionResult.InProgress;
	}
	
	public override void InitiateActionPlan(Agent correspondingAgent) {
		base.InitiateActionPlan(correspondingAgent);

		_worldCellAgentFeelsMostCertain = null;
	}

	public override bool CanBeExecuted(
		EnvironmentWorldCell currentEnvironmentWorldCell,
		List<EnvironmentWorldCell> agentsFieldOfView,
		List<Agent> nearbyAgents) {
		return nearbyAgents.Contains(_agentToFleeFrom);
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		double painAvoidanceValue = 1-hypothalamus.GetCurrentPainAvoidanceValue();
		
		double competenceCancellation = (hypothalamus.GetCurrentCompetenceValue() + GetSuccessProbability());
		return (painAvoidanceValue) / competenceCancellation;
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
		return 0;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return 0;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return 0;
	}
}