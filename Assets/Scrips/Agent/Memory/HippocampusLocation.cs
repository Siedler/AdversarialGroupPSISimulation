using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Priority_Queue;
using Scrips.Agent.Memory;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnionFind;
using UnityEngine;

public class HippocampusLocation {
	private readonly Agent _agent;
	
	private readonly Environment _environment;
	private readonly Dictionary<Vector3Int, AgentMemoryWorldCell> _agentLocationMemory;

	// Array that saves the forget rate for the location memory seperate for positive and negative values (double[2])
	private readonly double[][] _locationForgetRatePositiveNegative;
	private readonly double[] _locationMemoryAlphaFactor;

	private List<FoodCluster> _foodClusters;

	public HippocampusLocation(Agent agent, Environment environment, AgentPersonality agentPersonality) {
		this._agent = agent;

		this._environment = environment;

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

		_locationMemoryAlphaFactor = new double[] {
			agentPersonality.GetValue("LocationMemoryReceiveLocationPainAvoidanceInformationFactor"),
			agentPersonality.GetValue("LocationMemoryReceiveLocationEnergyInformationFactor"),
			agentPersonality.GetValue("LocationMemoryReceiveLocationAffiliationInformationFactor"),
			agentPersonality.GetValue("LocationMemoryReceiveLocationCertaintyInformationFactor"),
			agentPersonality.GetValue("LocationMemoryReceiveLocationCompetenceInformationFactor"),
		};
		
		_agentLocationMemory = new Dictionary<Vector3Int, AgentMemoryWorldCell>();

		_foodClusters = new List<FoodCluster>();
	}

	public bool KnowsLocation(Vector3Int gridCoordinates) {
		return _agentLocationMemory.ContainsKey(gridCoordinates);
	}

	public void AddNewWorldCellToMemory(Vector3Int gridCoordinates, AgentMemoryWorldCell agentMemoryWorldCell) {
		_agentLocationMemory.Add(gridCoordinates, agentMemoryWorldCell);
	}

	// This method is called if an agents receives new location information from another agent
	// The _locationMemoryAlphaFactor regulated by how much the new information is taken into account.
	// This factor is depended on the personality of the agent.
	public void ReceiveLocationInformation(Vector3Int gridCoordinates, double[] needSatisfactionAssociations) {
		_agentLocationMemory[gridCoordinates].UpdateNeedSatisfactionAssociations(
			needSatisfactionAssociations[0] * _locationMemoryAlphaFactor[0],
			needSatisfactionAssociations[1] * _locationMemoryAlphaFactor[1],
			needSatisfactionAssociations[2] * _locationMemoryAlphaFactor[2],
			needSatisfactionAssociations[3] * _locationMemoryAlphaFactor[3],
			needSatisfactionAssociations[4] * _locationMemoryAlphaFactor[4]
			);
	}
	
	public AgentMemoryWorldCell GetAgentMemoryWorldCell(Vector3Int gridCoordinates) {
		return _agentLocationMemory[gridCoordinates];
	}

	public Dictionary<Vector3Int, AgentMemoryWorldCell> GetAgentsLocationMemory() {
		return _agentLocationMemory;
	}

	private AgentMemoryWorldCell GetClosetAgentMemoryWorldCell(Vector3Int coordinate) {
		// If the center coordinate is inside the map return it
		if (_agentLocationMemory.ContainsKey(coordinate)) return _agentLocationMemory[coordinate];
		
		// If its not part of the map: search closest memory world cell
		AgentMemoryWorldCell closestSoFar = null;
		double closestDistanceSoFar = Double.PositiveInfinity;

		foreach ((Vector3Int agentMemoryWorldCellCoordiantes, AgentMemoryWorldCell agentMemoryWorldCell) in _agentLocationMemory) {
			double distance = HexagonGridUtility.GetHexGridDistance(coordinate, agentMemoryWorldCellCoordiantes);

			if (closestSoFar == null || distance < closestDistanceSoFar) {
				closestSoFar = agentMemoryWorldCell;
				closestDistanceSoFar = distance;
			}
		}

		return closestSoFar;
	}

	private DisjointSet<AgentMemoryWorldCell> FindClusters(double minFoodScoreForCluster) {
		DisjointSet<AgentMemoryWorldCell> disjointFoodClusters = new DisjointSet<AgentMemoryWorldCell>();

		// Itterate over every AgentMemoryWorldCell and search for clusters
		foreach (AgentMemoryWorldCell agentMemoryWorldCell in _agentLocationMemory.Values) {
			double foodAssociation = agentMemoryWorldCell.GetNeedSatisfactionAssociations()[1];

			if (foodAssociation >= minFoodScoreForCluster) {
				// Part of a cluster!
				disjointFoodClusters.MakeSet(agentMemoryWorldCell);
				
				AgentMemoryWorldCell[] neighbours = agentMemoryWorldCell.GetNeighbours();
				foreach (AgentMemoryWorldCell neighbour in neighbours) {
					if(neighbour == null) continue;
					
					// If neighbour is also part of a cluster -> Join them!
					if (disjointFoodClusters.ContainsData(neighbour)) {
						AgentMemoryWorldCell neighbourClusterRepresentative = disjointFoodClusters.FindSet(neighbour);
						AgentMemoryWorldCell ownClusterRepresentative = disjointFoodClusters.FindSet(agentMemoryWorldCell);
						disjointFoodClusters.Union(ownClusterRepresentative, neighbourClusterRepresentative);
					}
				}
			}
		}

		return disjointFoodClusters;
	}

	private Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>>
		CreateClusterDictionaryWithCenterRepresentative(Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>> foodClusters) {
		
		// Calculate the list of cluster centers
		Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>> clustersByCenter =
			new Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>>();
		
		foreach ((AgentMemoryWorldCell _, HashSet<AgentMemoryWorldCell> clusterMembers) in foodClusters) {
			Vector3 coordinateAverage = new Vector3();
			
			foreach (AgentMemoryWorldCell clusterMember in clusterMembers) {
				coordinateAverage += clusterMember.worldCoordinates;
			}
			coordinateAverage /= clusterMembers.Count;

			Vector3Int centerWorldCellCoordinate = _environment.GetCellCoordinateFromWorldCoordinate(coordinateAverage);
			AgentMemoryWorldCell centerWorldCell = GetClosetAgentMemoryWorldCell(centerWorldCellCoordinate);
			clustersByCenter.Add(centerWorldCell, clusterMembers);
		}

		return clustersByCenter;
	}
	
	public Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>> FindFoodCluster(double minFoodScoreForCluster = 0.3, double matchingDistanceThreshold = 4) {
		DisjointSet<AgentMemoryWorldCell> disjointFoodClusters = FindClusters(minFoodScoreForCluster);
		
		// Find new food clusters
		Dictionary<AgentMemoryWorldCell, HashSet<AgentMemoryWorldCell>> newFoodClusters =
			CreateClusterDictionaryWithCenterRepresentative(disjointFoodClusters.GetClusters());
		
		// Assign Food Clusters
		List<FoodCluster> currentFoodClusterCopy = new List<FoodCluster>(_foodClusters);
		List<AgentMemoryWorldCell> unassignedNewFoodClusters = new List<AgentMemoryWorldCell>(newFoodClusters.Keys);
		
		foreach (AgentMemoryWorldCell unassignedFoodClusterCenter in unassignedNewFoodClusters.ToArray()) {
			FoodCluster potentialMatch = null;
			double potentialMatchDistance = Double.PositiveInfinity;

			foreach (FoodCluster foodCluster in currentFoodClusterCopy) {
				if (foodCluster.GetCenter() == unassignedFoodClusterCenter) {
					potentialMatch = foodCluster;
					break;
				}
				
				double distance = HexagonGridUtility.GetHexGridDistance(foodCluster.GetCenter().cellCoordinates,
					unassignedFoodClusterCenter.cellCoordinates);
				
				if(distance > matchingDistanceThreshold || distance > potentialMatchDistance) continue;

				potentialMatch = foodCluster;
				potentialMatchDistance = distance;
			}
			
			if(potentialMatch == null) continue;
			
			if (potentialMatch.GetCenter() != unassignedFoodClusterCenter) {
				potentialMatch.SetCenter(unassignedFoodClusterCenter);
				currentFoodClusterCopy.Remove(potentialMatch);
			}

			unassignedNewFoodClusters.Remove(unassignedFoodClusterCenter);
		}

		// Add new food clusters
		foreach (AgentMemoryWorldCell unassignedNewFoodCluster in unassignedNewFoodClusters) {
			// Create new food cluster
			FoodCluster newFoodCluster = new FoodCluster(unassignedNewFoodCluster, newFoodClusters[unassignedNewFoodCluster]);
			_agent.AddNewFoodCluster(newFoodCluster);
			_foodClusters.Add(newFoodCluster);
		}

		// Remove food clusters
		foreach (FoodCluster foodCluster in currentFoodClusterCopy) {
			// Delete food cluster
			_foodClusters.Remove(foodCluster);
			_agent.RemoveFoodCluster(foodCluster);
		}
		
		return newFoodClusters;
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
	}

	public void Tick() {
		foreach (AgentMemoryWorldCell agentMemoryWorldCell in _agentLocationMemory.Values) {
			agentMemoryWorldCell.Forget(_locationForgetRatePositiveNegative);
		}
	}
}