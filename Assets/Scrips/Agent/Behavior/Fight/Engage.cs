using System;
using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;

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
		
		expectedPainAvoidance = GetOnSuccessPainAvoidanceSatisfaction();
		expectedEnergyIntake =  GetOnSuccessEnergySatisfaction();
		expectedAffiliation = GetOnSuccessAffiliationSatisfaction();
		expectedCertainty = GetOnSuccessCertaintySatisfaction();
		expectedCompetence = GetOnSuccessCompetenceSatisfaction();
	}

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();
		
		// If the action plan was enacted as the response to the request of another agent: reward socially
		if (_requestedHelp) {
			_agentThatCalledForHelp.ReceivedHelpAfterCalling(agent);
			socialMemory.SocialInfluence(_agentThatCalledForHelp, 0.1);
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
				
				_eventHistoryManager.AddHistoryEvent("Attacking " + _agentToAttack.name + "!");

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
		return SimulationSettings.EngageOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.EngageOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		// The friendly modifier is 2 if the agents get well along and therefore the punishment for asocial behaviour
		// is high
		double friendlyModifier = socialMemory.GetSocialScore(_agentToAttack) + 1;
		return SimulationSettings.EngageOnSuccess[2] * friendlyModifier;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		// Calculate the modifier for the certainty increase/decrease based on the social score.
		// This is modeled after a simple -x^3 equation to simulate, that the certainty increases for agents with
		// low social scores and decreases for agents with a high social score
		double friendlyModifier = -Math.Pow(socialMemory.GetSocialScore(_agentToAttack), 3);
		return SimulationSettings.EngageOnSuccess[3] * friendlyModifier;
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.EngageOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.EngageOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.EngageOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		double friendlyModifier = socialMemory.GetSocialScore(_agentToAttack) + 1;
		return SimulationSettings.EngageOnFailure[2] * friendlyModifier;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.EngageOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.EngageOnFailure[4];
	}

	public void RequestHelpToAttackThisAgent(Agent agentThatCalledForHelp) {
		_agentThatCalledForHelp = agentThatCalledForHelp;
		_requestedHelp = true;
	}
}