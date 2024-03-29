﻿using System.Collections.Generic;
using Scrips.Agent;
using Scrips.Agent.Personality;
using UnityEngine;

public class ExchangeLocationInformation : ActionPlan {
	private readonly Agent _correspondingAgent;

	private bool _calledOutRequest = false;
	private bool _wasRequested = false;

	public ExchangeLocationInformation(
		Agent agent,
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

	public override void InitiateActionPlan() {
		base.InitiateActionPlan();
		_calledOutRequest = false;

		_eventHistoryManager.AddHistoryEvent("Starting ActionPlan to exchange location information with " +
		                                     _correspondingAgent.name);
	}

	private void GiveLocationInformation() {
		// TODO maybe develop more complex location exchange
		// such as: exchanging information about unknown places of the other agent, etc.
		Dictionary<Vector3Int, AgentMemoryWorldCell> agentsLocationMemory = locationMemory.GetAgentsLocationMemory();
		List<Vector3Int> knownLocationsCoordinateSet = new List<Vector3Int>(agentsLocationMemory.Keys);
		
		// Get SimulationSettings.NumberOfWorldCellsToExchange random world cells to exchange if possible
		List<Vector3Int> locationsToExchange = new List<Vector3Int>();
		int i = 0;
		while (i < SimulationSettings.NumberOfWorldCellsToExchange && knownLocationsCoordinateSet.Count > 0) {
			int randomIndex = Random.Range(0, locationsToExchange.Count);
			locationsToExchange.Add(knownLocationsCoordinateSet[randomIndex]);
			knownLocationsCoordinateSet.RemoveAt(randomIndex);
			
			i++;
		}

		// Get all associated needSatisfactionScores and add them to a dictionary to exchange later
		Dictionary<Vector3Int, double[]> needSatisfactionAssociations = new Dictionary<Vector3Int, double[]>();
		for (int j = 0; j < locationsToExchange.Count; j++) {
			needSatisfactionAssociations.Add(locationsToExchange[j], agentsLocationMemory[locationsToExchange[j]].GetNeedSatisfactionAssociations());
		}
		
		// Give information
		_correspondingAgent.ReceiveLocationMemory(needSatisfactionAssociations, agent);
		_eventHistoryManager.AddHistoryEvent("Gave " + _correspondingAgent.name + " location information.");
	}

	public override ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		// Call out for the agent that you want to exchange information with
		if (!_calledOutRequest) {
			CallOutRequest(RequestType.InformationLocation, agentsFieldOfView, _correspondingAgent);
			_calledOutRequest = true;
		}
		
		int agentInRange = IsAgentInRange(agentsFieldOfView, _correspondingAgent);

		if (agentInRange == -1) {
			EnvironmentWorldCell environmentWorldCellOfAgentToExchangeInformation =
				IsAgentInFieldOfView(agentsFieldOfView, _correspondingAgent);

			if (environmentWorldCellOfAgentToExchangeInformation == null) {
				_eventHistoryManager.AddHistoryEvent("Failed to give " + _correspondingAgent.name + " location information :(");
				
				OnFailure();
				return ActionResult.Failure;
			}

			WalkTo(environmentWorldCellOfAgentToExchangeInformation.cellCoordinates);

			// Recheck if agent is now in range. If so -> give information
			agentInRange = IsAgentInRange(agentsFieldOfView, _correspondingAgent);
			if (agentInRange != -1) {
				SuccessCall();
				return ActionResult.Success;
			}

			return ActionResult.InProgress;
		}

		// Agent is in range -> Exchange Social Information
		SuccessCall();
		return ActionResult.Success;
	}

	// Combining everything that has to be done on success
	private void SuccessCall() {
		GiveLocationInformation();
		OnSuccess();
		Statistics._current.LogSocialEvent(agent, _correspondingAgent, TimeManager.current.GetCurrentTimeStep(), SocialEventType.ExchangeLocationInformation);
	}

	public override bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		_wasRequested = _wasRequested && nearbyAgents.Contains(_correspondingAgent);
		return nearbyAgents.Contains(_correspondingAgent);
	}

	public override double GetUrgency(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents) {
		// Give the urgency by the land of _wasRequested, i.e. if the other agent asked for the informational exchange,
		// and if the other agent is still in the field of view.
		// If the agent goes out of the field of view then the _wasRequested is set to false
		return (_wasRequested = (_wasRequested && nearbyAgents.Contains(_correspondingAgent))) ? 0.1 : 0;
	}

	protected override double GetOnSuccessPainAvoidanceSatisfaction() {
		return SimulationSettings.ExchangeInformationOnSuccess[0];
	}

	protected override double GetOnSuccessEnergySatisfaction() {
		return SimulationSettings.ExchangeInformationOnSuccess[1];
	}

	protected override double GetOnSuccessAffiliationSatisfaction() {
		// The friendly modifier is 2 if the agents get well along and therefore the punishment for asocial behaviour
		// is high
		double friendlyModifier = socialMemory.GetSocialScore(_correspondingAgent) + 1;
		return SimulationSettings.ExchangeInformationOnSuccess[2] * friendlyModifier;
	}

	protected override double GetOnSuccessCertaintySatisfaction() {
		return SimulationSettings.ExchangeInformationOnSuccess[3];
	}

	protected override double GetOnSuccessCompetenceSatisfaction() {
		return SimulationSettings.ExchangeInformationOnSuccess[4];
	}

	protected override double GetOnFailurePainAvoidanceSatisfaction() {
		return SimulationSettings.ExchangeInformationOnFailure[0];
	}

	protected override double GetOnFailureEnergySatisfaction() {
		return SimulationSettings.ExchangeInformationOnFailure[1];
	}

	protected override double GetOnFailureAffiliationSatisfaction() {
		return SimulationSettings.ExchangeInformationOnFailure[2];
	}

	protected override double GetOnFailureCertaintySatisfaction() {
		return SimulationSettings.ExchangeInformationOnFailure[3];
	}

	protected override double GetOnFailureCompetenceSatisfaction() {
		return SimulationSettings.ExchangeInformationOnFailure[4];
	}
	
	public void RegisterRequest() {
		_wasRequested = true;
	}
}