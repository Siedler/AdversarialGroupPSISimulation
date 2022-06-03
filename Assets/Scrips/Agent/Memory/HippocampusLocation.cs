using System;
using System.Collections.Generic;
using System.IO;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using Unity.Mathematics;
using UnityEngine;

public class HippocampusLocation {
	private Dictionary<Vector3Int, AgentMemoryWorldCell> _agentLocationMemory;

	// Array that saves the forget rate for the location memory seperate for positive and negative values (double[2])
	private double[][] _locationForgetRatePositiveNegative;

	public HippocampusLocation(AgentPersonality agentPersonality) {
		_locationForgetRatePositiveNegative = new double[][] {
			new double[] {
				agentPersonality.GetValue("HippocampusLocationPainAvoidanceForgetRatePositive"),
				agentPersonality.GetValue("HippocampusLocationEnergyForgetRatePositive"),
				agentPersonality.GetValue("HippocampusLocationAffiliationForgetRatePositive"),
				agentPersonality.GetValue("HippocampusLocationCertaintyForgetRatePositive"),
				agentPersonality.GetValue("HippocampusLocationCompetenceForgetRatePositive"),
				
			},
			new double[] {
				agentPersonality.GetValue("HippocampusLocationPainAvoidanceForgetRateNegative"),
				agentPersonality.GetValue("HippocampusLocationEnergyForgetRateNegative"),
				agentPersonality.GetValue("HippocampusLocationAffiliationForgetRateNegative"),
				agentPersonality.GetValue("HippocampusLocationCertaintyForgetRateNegative"),
				agentPersonality.GetValue("HippocampusLocationCompetenceForgetRateNegative")
			},
		};
		
		_agentLocationMemory = new Dictionary<Vector3Int, AgentMemoryWorldCell>();
	}

	public bool KnowsLocation(Vector3Int gridCoordinates) {
		return _agentLocationMemory.ContainsKey(gridCoordinates);
	}

	public void AddNewWorldCellToMemory(Vector3Int gridCoordinates, AgentMemoryWorldCell agentMemoryWorldCell) {
		_agentLocationMemory.Add(gridCoordinates, agentMemoryWorldCell);
	}
	
	public AgentMemoryWorldCell GetAgentMemoryWorldCell(Vector3Int gridCoordinates) {
		return _agentLocationMemory[gridCoordinates];
	}

	public Dictionary<Vector3Int, AgentMemoryWorldCell> GetAgentsLocationMemory() {
		return _agentLocationMemory;
	}

	public void UpdateNeedSatisfactionAssociations(
		Vector3Int agentMemoryWorldCellCoordinate,
		double painAvoidanceSatisfaction,
		double energySatisfaction,
		double affiliationSatisfaction,
		double certaintySatisfaction,
		double competenceSatisfaction) {

		if (!_agentLocationMemory.ContainsKey(agentMemoryWorldCellCoordinate)) {
			throw new InvalidDataException("The world cell coordinate + " + agentMemoryWorldCellCoordinate +
			                               " is not know to the Agent!");
		}
			
		List<Vector3Int> circleAroundCenterMemoryCell = HexagonGridUtility.GetGridCellCircleCoordinates(
			agentMemoryWorldCellCoordinate, SimulationSettings.MemoryWorldCellNeedSatisfactionAssociationRadius);

		// Update need satisfaction associations for the current world cell
		_agentLocationMemory[agentMemoryWorldCellCoordinate].UpdateNeedSatisfactionAssociations(
			painAvoidanceSatisfaction, energySatisfaction, affiliationSatisfaction, certaintySatisfaction,
			competenceSatisfaction);
		
		for (int i = 0; i < SimulationSettings.MemoryWorldCellNeedSatisfactionAssociationRadius; i++) {
			double reductionFactor = 1 - ((double) (i+1) / (SimulationSettings.MemoryWorldCellNeedSatisfactionAssociationRadius+1));

			double painAvoidanceSatisfactionFactored = painAvoidanceSatisfaction * reductionFactor;
			double energySatisfactionFactored = energySatisfaction * reductionFactor;
			double affiliationSatisfactionFactored = affiliationSatisfaction * reductionFactor;
			double certaintySatisfactionFactored = certaintySatisfaction * reductionFactor;
			double competenceSatisfactionFactored = competenceSatisfaction * reductionFactor;
							
			for (int j = 6*MathHelper.GaussianSum(i); j < 6*MathHelper.GaussianSum(i+1); j++) {
				if(!_agentLocationMemory.ContainsKey(circleAroundCenterMemoryCell[j])) continue;
				
				_agentLocationMemory[circleAroundCenterMemoryCell[j]].UpdateNeedSatisfactionAssociations(
					painAvoidanceSatisfactionFactored,
					energySatisfactionFactored,
					affiliationSatisfactionFactored,
					certaintySatisfactionFactored,
					competenceSatisfactionFactored
				);
			}
		}
		Debug.Log("");
	}

	public void Tick() {
		foreach (AgentMemoryWorldCell agentMemoryWorldCell in _agentLocationMemory.Values) {
			agentMemoryWorldCell.Forget(_locationForgetRatePositiveNegative);
		}
	}
}