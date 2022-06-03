using System.Collections.Generic;
using Scrips.Agent;
using UnityEngine;

public class Engage : ActionPlan {
	private Agent _agentToAttack;

	public Engage(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		Environment environment, Agent agentToAttack) : base(agent, hypothalamus, locationMemory, socialMemory, environment) {

		_agentToAttack = agentToAttack;
		
		expectedPainAvoidance = -0.2;
		expectedEnergyIntake = 0;
		expectedAffiliation = agent.GetTeam() == agentToAttack.GetTeam() ? -0.5 : -0.1;
		expectedCertainty = agent.GetTeam() == agentToAttack.GetTeam() ? -0.3 : 0.8;
		expectedCompetence = agent.GetTeam() == agentToAttack.GetTeam() ? 0.4 : 0.8;
	}

	public override void InitiateActionPlan(Agent correspondingAgent = null) {
		base.InitiateActionPlan(correspondingAgent);
		
		_agentToAttack = correspondingAgent;
	}

	private ActionResult Hit(List<EnvironmentWorldCell> agentsFieldOfView) {
		EnvironmentWorldCell environmentWorldCellToAttack = null;
		int i;
		// Check one tile around agent
		for (i = 0; i < 6; i++) {
			if(agentsFieldOfView[i] == null || agentsFieldOfView[i].GetAgent() != _agentToAttack) continue;

			environmentWorldCellToAttack = agentsFieldOfView[i];
			break;
		}

		if (environmentWorldCellToAttack == null) {
			return ActionResult.Failure;
		}

		_agentToAttack.TakeDamage(10, agent);
		agent.SetOrientation((Direction) i);
		
		return ActionResult.Success;
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {

		if (_agentToAttack == null) {
			_agentToAttack = GetWorstEnemyAgentInFieldOfView(agentsFieldOfView);
			if (_agentToAttack == null) return ActionResult.Failure;
		}

		for (int i = 0; i < agentsFieldOfView.Count; i++) {
			EnvironmentWorldCell environmentWorldCell = agentsFieldOfView[i];
			
			if(environmentWorldCell == null || !environmentWorldCell.IsOccupied() || environmentWorldCell.GetAgent() != _agentToAttack) continue;

			if (i < 6) {
				Hit(agentsFieldOfView);
				
				Debug.Log(agent.name + " attacks " + _agentToAttack.name);

				if (!environmentWorldCell.IsOccupied()) {
					OnSuccess();
					return ActionResult.Success;
				}

				return ActionResult.InProgress;
			}

			WalkTo(environmentWorldCell.cellCoordinates);
			return ActionResult.InProgress;
		}

		OnFailure();
		return ActionResult.Failure;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return nearbyAgents.Contains(_agentToAttack);
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
		return 0.5;
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return 0.8;
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
		return -0.5;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.5;
	}
}