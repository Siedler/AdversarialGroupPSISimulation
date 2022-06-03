using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Priority_Queue;
using Scrips;
using Scrips.Agent;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding {

	public static (List<Direction>, List<WorldCell>) SimpleAStarPathFinding(
		Vector3Int currentWorldCellCoordinates,
		Vector3Int destinationWorldCellCoordinates,
		Dictionary<Vector3Int, EnvironmentWorldCell> environmentMap,
		Func<Vector3Int, Vector3Int, float> distanceHeuristic = null) {

		if (distanceHeuristic == null) {
			distanceHeuristic = DistanceHeuristic;
		}
		
		if (!environmentMap.ContainsKey(currentWorldCellCoordinates)
		    || !environmentMap.ContainsKey(destinationWorldCellCoordinates)) {
			throw new ArgumentException("One or both provided coordinates do not exist!");
		}

		// Check if start- and endpoint are the same. If so return an empty path
		if (currentWorldCellCoordinates == destinationWorldCellCoordinates) {
			return (new List<Direction>(), new List<WorldCell>());
		}
		
		Dictionary<WorldCell, WorldCellAStar> fastPriorityNodes = new Dictionary<WorldCell, WorldCellAStar>();

		HashSet<WorldCellAStar> alreadyVisitedWorldCells = new HashSet<WorldCellAStar>();
		FastPriorityQueue<WorldCellAStar> priorityQueue = new FastPriorityQueue<WorldCellAStar>(environmentMap.Count);

		
		// Initiate algorithm
		WorldCell startEnvironmentWorldCell = environmentMap[currentWorldCellCoordinates];
		WorldCellAStar startAStarCell = new WorldCellAStar(startEnvironmentWorldCell, null, Direction.Unspecified, 0);
		fastPriorityNodes.Add(startEnvironmentWorldCell, startAStarCell);
		priorityQueue.Enqueue(startAStarCell, 0);

		while (priorityQueue.Count > 0) {
			WorldCellAStar nextCell = priorityQueue.Dequeue();
			alreadyVisitedWorldCells.Add(nextCell);

			if (nextCell.GetWorldCell().cellCoordinates == destinationWorldCellCoordinates) {
				// Found goal!!!
				List<Direction> directions = new List<Direction>();
				List<WorldCell> worldCells = new List<WorldCell>();
				
				for (WorldCellAStar i = nextCell; i.GetPreviousWorldCell() != null; i = i.GetPreviousWorldCell()) {
					directions.Add(i.GetPreviousDirection());
					worldCells.Add(i.GetPreviousWorldCell().GetWorldCell());
				}
				
				directions.Reverse();
				worldCells.Reverse();
				
				return (directions, worldCells);
			}

			WorldCell[] neighbours = nextCell.GetWorldCell().GetNeighbours();

			for (int i = 0; i < neighbours.Length; i++) {
				if(neighbours[i] == null) continue;
				
				WorldCell neighbour = neighbours[i];

				float realDistanceFromStar = nextCell.GetDistanceFromStart() + 1; 
				float priority =  realDistanceFromStar + distanceHeuristic(neighbour.cellCoordinates, destinationWorldCellCoordinates);

				// Check if cell is already part of the priority queue
				if (fastPriorityNodes.ContainsKey(neighbour)) {
					WorldCellAStar neighbouringAStarWorldCell = fastPriorityNodes[neighbour];
					
					// If the neighbouring world cell was already visited: skip to the next
					if(alreadyVisitedWorldCells.Contains(neighbouringAStarWorldCell)) continue;
					
					if (priority < neighbouringAStarWorldCell.Priority) {
						neighbouringAStarWorldCell.SetPreviousWorldCell(nextCell, (Direction) i, realDistanceFromStar);
						priorityQueue.UpdatePriority(neighbouringAStarWorldCell, priority);
					}
					
					continue;
				}
				
				// WorldCell is new
				WorldCellAStar newAStarWorldCell =
					new WorldCellAStar(neighbour, nextCell, (Direction) i, realDistanceFromStar);
				fastPriorityNodes.Add(neighbour, newAStarWorldCell);
				priorityQueue.Enqueue(newAStarWorldCell, priority);
			}
		}
		
		return (null, null);
	}

	public static List<Direction> SimpleAStarPathFindingDirections(Vector3Int currentWorldCellCoordinates,
		Vector3Int destinationWorldCellCoordinates, Dictionary<Vector3Int, EnvironmentWorldCell> environmentMap, Func<Vector3Int, Vector3Int, float> distanceHeuristic = null) {
		return SimpleAStarPathFinding(currentWorldCellCoordinates, destinationWorldCellCoordinates, environmentMap,
			distanceHeuristic).Item1;
	}
	
	public static List<WorldCell> SimpleAStarPathFindingWorldCells(Vector3Int currentWorldCellCoordinates,
		Vector3Int destinationWorldCellCoordinates, Dictionary<Vector3Int, EnvironmentWorldCell> environmentMap, Func<Vector3Int, Vector3Int, float> distanceHeuristic = null) {
		return SimpleAStarPathFinding(currentWorldCellCoordinates, destinationWorldCellCoordinates, environmentMap,
			distanceHeuristic).Item2;
	}
	
	public static float DistanceHeuristic(Vector3Int currentCoordinates, Vector3Int goalCoordinates) {
		// TODO implement
		return 100;
	}
	
}