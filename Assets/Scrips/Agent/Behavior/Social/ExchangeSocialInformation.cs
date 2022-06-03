﻿using System;
using System.Collections.Generic;
using Scrips.Agent;

public class ExchangeSocialInformation : ActionPlan {

	private readonly Agent _correspondingAgent;

	public ExchangeSocialInformation(
		Agent agent,
		Hypothalamus hypothalamus,
		HippocampusLocation locationMemory,
		HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager,
		Environment environment,
		Agent correspondingAgent) : base(agent, hypothalamus, locationMemory, socialMemory, eventHistoryManager, environment) {

		_correspondingAgent = correspondingAgent;
		
		expectedPainAvoidance = 0;
		expectedEnergyIntake = 0;
		expectedAffiliation = 0.5;
		expectedCertainty = 0.3;
		expectedCompetence = 0.5;
	}

	private void GiveSocialInformation() {
		(Agent, AgentIndividualMemory) agentIndividualMemoryPair = socialMemory.GetRandomAgentMemory(_correspondingAgent);
		_correspondingAgent.ReceiveAgentIndividualMemory(agentIndividualMemoryPair.Item1,
			agentIndividualMemoryPair.Item2.GetSocialScore());
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
				GiveSocialInformation();
				OnSuccess();
				return ActionResult.Success;
			}
			
			return ActionResult.InProgress;
		}
		
		// Agent is in range -> Exchange Social Information
		GiveSocialInformation();
		OnSuccess();
		return ActionResult.Success;
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		return nearbyAgents.Contains(_correspondingAgent);
	}
	
	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return 0;
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		return 0.5;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return 0.3;
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
		return -0.1;
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return 0;
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return -0.5;
	}
}