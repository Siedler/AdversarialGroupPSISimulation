using System;
using System.Collections.Generic;
using System.Linq;
using Scrips;
using Scrips.Agent;
using Scrips.Helper.Math;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ActionPlan 
{
	protected Agent agent;

	protected AgentEventHistoryManager _eventHistoryManager;
	
	protected Hypothalamus hypothalamus;

	protected readonly HippocampusLocation locationMemory;
	protected readonly HippocampusSocial socialMemory;
	
	protected readonly Environment environment;

	private Vector3Int _prevDestination;
	private List<Direction> _pathToWalk;
	
	protected double successProbability;
	// TODO Maybe unite these values into a single array?
	protected double expectedPainAvoidance;
	protected double expectedEnergyIntake;
	protected double expectedAffiliation;
	protected double expectedCertainty;
	protected double expectedCompetence;
	
	public ActionPlan(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory, HippocampusSocial socialMemory,
		AgentEventHistoryManager eventHistoryManager, Environment environment) {
		this.agent = agent;

		this._eventHistoryManager = eventHistoryManager;

		this.hypothalamus = hypothalamus;
		
		this.locationMemory = locationMemory;
		this.socialMemory = socialMemory;

		this.environment = environment;
	}

	public virtual void InitiateActionPlan(Agent correspondingAgent = null) {
		_pathToWalk = null;
	}
	
	public abstract ActionResult Execute(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents);

	public abstract bool CanBeExecuted(EnvironmentWorldCell currentEnvironmentWorldCell, List<EnvironmentWorldCell> agentsFieldOfView, List<Agent> nearbyAgents);
	
	protected ActionResult CallOutRequest(RequestType requestType, List<EnvironmentWorldCell> agentsFieldOfView, Agent regardingAgent = null) {

		RequestInformation requestInformation =
			new RequestInformation(requestType, agent, regardingAgent, agent.GetCurrentWorldCell());

		// Callout to all agents in sensing field / i.e. field of view
		// TODO maybe change seeing and hearing distance?
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if(environmentWorldCell == null || !environmentWorldCell.IsOccupied()) continue;

			environmentWorldCell.GetAgent().RegisterIncomingRequest(requestInformation);
		}
		
		return ActionResult.Success;
	}
	
	// TODO 
	protected ActionResult WalkTo(Vector3Int destinationCellCoordinate) {
		if (_pathToWalk == null || _prevDestination != destinationCellCoordinate) {
			_prevDestination = destinationCellCoordinate;

			_pathToWalk = FindPath(destinationCellCoordinate);
			if (_pathToWalk == null) throw new InvalidOperationException("No path could be found for the given coordinate!");
		}

		if (_pathToWalk.Count == 0) return ActionResult.Success;

		Direction nextDirection = _pathToWalk[0];

		Vector3Int newCoordinate =
			HexagonGridUtility.GetCoordinatesOfNeighbouringCell(agent.GetCurrentWorldCell().cellCoordinates, nextDirection);

		if (!environment.DoesCellWithCoordinateExist(newCoordinate)) { 
			new InvalidOperationException("The cell with coordinate " + newCoordinate + " with direction " + nextDirection + " that the agent " + agent.name + " wanted to navigate to does not exist!");
		}

		EnvironmentWorldCell newEnvironmentWorldCell = environment.GetWorldCellByCoordinates(newCoordinate);
        
		// If the world cell is occupied => Do nothing
		// TODO Improve evasion method
		// IDEA: Agent with less competence evades
		if (newEnvironmentWorldCell.IsOccupied()) {
			bool shouldStartAversion = Random.Range(0f, 1f) > 0.5;
			
			if (!shouldStartAversion) return ActionResult.InProgress;
			
			Vector3Int[] neighbouringWorldCells = HexagonGridUtility.GetCoordinatesOfNeighbouringCells(agent.GetCurrentWorldCell().cellCoordinates);
			List<Tuple<EnvironmentWorldCell, Direction>> emptyWorldCells = new List<Tuple<EnvironmentWorldCell, Direction>>();
			
			// Remove 
			for (int i = 0; i < neighbouringWorldCells.Length; i++) {
				EnvironmentWorldCell neighbouringWorldCell =
					environment.GetWorldCellByCoordinates(neighbouringWorldCells[i]);
				
				if (neighbouringWorldCell == null || neighbouringWorldCell.IsOccupied()) continue;
				emptyWorldCells.Add(new Tuple<EnvironmentWorldCell, Direction>(neighbouringWorldCell, (Direction) i));
			}

			if (emptyWorldCells.Count == 0) return ActionResult.InProgress;
			
			Tuple<EnvironmentWorldCell, Direction> randomEvasion =
				emptyWorldCells[Random.Range(0, emptyWorldCells.Count)];

			agent.ChangeCurrentWorldCell(randomEvasion.Item1, randomEvasion.Item2);
			
			_pathToWalk = FindPath(destinationCellCoordinate);
			if (_pathToWalk == null) throw new InvalidOperationException("No path could be found for the given coordinate!");
			return ActionResult.InProgress;
		}

		agent.ChangeCurrentWorldCell(newEnvironmentWorldCell, nextDirection);
		
		// Remove direction from list
		_pathToWalk.RemoveAt(0);
		
		return _pathToWalk.Count == 0 ? ActionResult.Success : ActionResult.InProgress;
	}
	
	private List<Direction> FindPath(Vector3Int cellCoordinates) {
		return PathFinding.SimpleAStarPathFindingDirections(agent.GetCurrentWorldCell().cellCoordinates,
			cellCoordinates, environment.GetEnvironmentMap());
	}

	protected Agent GetWorstEnemyAgentInFieldOfView(List<EnvironmentWorldCell> agentsFieldOfView) {
		Agent worstEnemyInRange = null;
		AgentIndividualMemory worstEnemyScore = null;
		
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if(environmentWorldCell == null || !environmentWorldCell.IsOccupied()) continue;

			Agent agentOnWorldCell = environmentWorldCell.GetAgent();
			AgentIndividualMemory memoryOfAgentOnWorldCell = agent.GetIndividualMemory(agentOnWorldCell);
			
			if(memoryOfAgentOnWorldCell.GetSocialScore() > 0) continue;
			if(worstEnemyScore != null && worstEnemyScore.GetSocialScore() < memoryOfAgentOnWorldCell.GetSocialScore()) continue;

			worstEnemyInRange = agentOnWorldCell;
			worstEnemyScore = memoryOfAgentOnWorldCell;
		}

		return worstEnemyInRange;
	}

	protected static EnvironmentWorldCell GetClosestFoodLocationInFieldOfView(List<EnvironmentWorldCell> agentsFieldOfView) {
		return agentsFieldOfView.FirstOrDefault(environmentWorldCell => environmentWorldCell != null && environmentWorldCell.ContainsFood());
	}
	
	protected static EnvironmentWorldCell IsAgentInFieldOfView(List<EnvironmentWorldCell> agentsFieldOfView,
		Agent agentToLookFor) {
		foreach (EnvironmentWorldCell environmentWorldCell in agentsFieldOfView) {
			if (environmentWorldCell == null || !environmentWorldCell.IsOccupied() ||
			    environmentWorldCell.GetAgent() != agentToLookFor) continue;

			return environmentWorldCell;
		}

		return null;
	}
	
	protected static int IsAgentInRange(List<EnvironmentWorldCell> agentsFieldOfView, Agent agentToLookFor) {
		for (int i = 0; i < 6; i++) {
			EnvironmentWorldCell environmentWorldCell = agentsFieldOfView[i];
			
			if (environmentWorldCell == null || !environmentWorldCell.IsOccupied() ||
			    environmentWorldCell.GetAgent() != agentToLookFor) continue;

			return i;
		}

		return -1;
	}

	public double GetSuccessProbability() {
		return successProbability;
	}

	public double[] GetExpectedSatisfaction() {
		double[] expectedSatisfaction = new double[] {
			expectedPainAvoidance,
			expectedEnergyIntake,
			expectedAffiliation,
			expectedCertainty,
			expectedCompetence
		};

		return expectedSatisfaction;
	}
	
	protected void OnSuccess() {
		successProbability = MathHelper.RunningAverage(successProbability, 1, 0.3f);

		double painAvoidanceSatisfaction = GetOnSuccessPainAvoidanceSatisfaction();
		double energySatisfaction = GetOnSuccessEnergySatisfaction();
		double affiliationSatisfaction = GetOnSuccessAffiliationSatisfaction();
		double certaintySatisfaction = GetOnSuccessCertaintySatisfaction();
		double competenceSatisfaction = GetOnSuccessCompetenceSatisfaction();
		
		expectedPainAvoidance = MathHelper.RunningAverage(expectedPainAvoidance, painAvoidanceSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedEnergyIntake = MathHelper.RunningAverage(expectedEnergyIntake, energySatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedAffiliation = MathHelper.RunningAverage(expectedAffiliation, affiliationSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedCertainty = MathHelper.RunningAverage(expectedCertainty, certaintySatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedCompetence = MathHelper.RunningAverage(expectedCompetence, competenceSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		
		agent.Experience(painAvoidanceSatisfaction, energySatisfaction, affiliationSatisfaction, certaintySatisfaction, competenceSatisfaction);
	}

	protected void OnFailure() {
		successProbability = MathHelper.RunningAverage(successProbability, 0, 0.3f);
		
		double painAvoidanceSatisfaction = GetOnFailurePainAvoidanceSatisfaction();
		double energySatisfaction = GetOnFailureEnergySatisfaction();
		double affiliationSatisfaction = GetOnFailureAffiliationSatisfaction();
		double certaintySatisfaction = GetOnFailureCertaintySatisfaction();
		double competenceSatisfaction = GetOnFailureCompetenceSatisfaction();
		
		expectedPainAvoidance = MathHelper.RunningAverage(expectedPainAvoidance, painAvoidanceSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedEnergyIntake = MathHelper.RunningAverage(expectedEnergyIntake, energySatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedAffiliation = MathHelper.RunningAverage(expectedAffiliation, affiliationSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedCertainty = MathHelper.RunningAverage(expectedCertainty, certaintySatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		expectedCompetence = MathHelper.RunningAverage(expectedCompetence, competenceSatisfaction, SimulationSettings.ActionPlanRollingAverageAlpha);
		
		agent.Experience(painAvoidanceSatisfaction, energySatisfaction, affiliationSatisfaction, certaintySatisfaction, competenceSatisfaction);
	}

	protected abstract double GetOnSuccessPainAvoidanceSatisfaction();
	protected abstract double GetOnSuccessEnergySatisfaction();
	protected abstract double GetOnSuccessAffiliationSatisfaction();
	protected abstract double GetOnSuccessCertaintySatisfaction();
	protected abstract double GetOnSuccessCompetenceSatisfaction();

	protected abstract double GetOnFailurePainAvoidanceSatisfaction();
	protected abstract double GetOnFailureEnergySatisfaction();
	protected abstract double GetOnFailureAffiliationSatisfaction();
	protected abstract double GetOnFailureCertaintySatisfaction();
	protected abstract double GetOnFailureCompetenceSatisfaction();
}