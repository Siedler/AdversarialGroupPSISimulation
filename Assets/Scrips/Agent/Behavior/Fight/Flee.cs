using System.Collections.Generic;
using Priority_Queue;
using Scrips.Agent;
using Scrips.Agent.Personality;

public class Flee : ActionPlan {

	private Agent _agentToFleeFrom;
	private AgentMemoryWorldCell _worldCellAgentFeelsMostCertain;

	private bool _calledForHelp = false;
	
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
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake =  GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();

		_worldCellAgentFeelsMostCertain = null;
		_calledForHelp = false;
		
		_eventHistoryManager.AddHistoryEvent("Fleeing from " + _agentToFleeFrom.name);
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
		if (!_calledForHelp) {
			CallOutRequest(RequestType.Help, agentsFieldOfView, _agentToFleeFrom);
			_calledForHelp = true;
		}
		
		if (!nearbyAgents.Contains(_agentToFleeFrom)) {
			OnSuccess();
			return ActionResult.Success;
		}

		if (_worldCellAgentFeelsMostCertain == null) {
			_worldCellAgentFeelsMostCertain = GetAgentMemoryWorldCellToFleeTo(currentEnvironmentWorldCell);

			_eventHistoryManager.AddHistoryEvent("Fleeing to " + _worldCellAgentFeelsMostCertain.cellCoordinates);
			
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
		return SimulationSettings.FleeOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.FleeOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return SimulationSettings.FleeOnSuccess[2];
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.FleeOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.FleeOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.FleeOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.FleeOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.FleeOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.FleeOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.FleeOnFailure[4];
	}
}