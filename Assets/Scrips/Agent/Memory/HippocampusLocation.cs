

using System.Collections.Generic;
using UnityEngine;

public class HippocampusLocation {
	private Dictionary<Vector3Int, AgentMemoryWorldCell> _agentLocationMemory;

	public HippocampusLocation() {
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
}