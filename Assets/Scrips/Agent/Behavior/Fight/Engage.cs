using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnityEngine;
using Random = System.Random;

public class Engage : ActionPlan {
	private Agent _agentToAttack;

	private Agent _agentThatCalledForHelp;
	private bool _requestedHelp;

	public Engage(
		Agent agent,
		AgentPersonality agentPersonality,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment, Agent agentToAttack) : base(agent, agentPersonality, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

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

		// If the action plan was enacted as the response to the request of another agent: reward socially
		if (_requestedHelp) {
			_agentThatCalledForHelp.ReceivedHelpAfterCalling(agent);
		}
	}

	private double GetDamageAmount() {
		return MathHelper.NextGaussianFromInterval(SimulationSettings.HitMinDamage, SimulationSettings.HitMaxDamage);
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

		double damage = GetDamageAmount();
		
		_agentToAttack.TakeDamage(damage, agent);
		agent.SetOrientation((Direction) i);
		
		socialMemory.SocialInfluence(_agentToAttack, -0.1);
		
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
				
				_eventHistoryManager.AddHistoryEvent("Agent " + agent.name + ": Attacking " + _agentToAttack.name + "!");

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
		_requestedHelp = _requestedHelp && nearbyAgents.Contains(_agentToAttack);
		return nearbyAgents.Contains(_agentToAttack);
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		_requestedHelp = _requestedHelp && nearbyAgents.Contains(_agentToAttack);
		double socialScore = socialMemory.GetIndividualMemory(_agentToAttack).GetSocialScore();
		
		// TODO maybe change socialMemory.GetIndividualMemory(_agentToAttack).GetSocialScore() > 0 to individual limit
		if (!_requestedHelp || socialScore > 0) return 0;

		return 0.1 * (-socialScore);
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

	public void RequestHelpToAttackThisAgent(Agent agentThatCalledForHelp) {
		_agentThatCalledForHelp = agentThatCalledForHelp;
		_requestedHelp = true;
	}
}