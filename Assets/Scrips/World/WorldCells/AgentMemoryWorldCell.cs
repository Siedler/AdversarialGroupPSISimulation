using Scrips.Agent;
using Unity.VisualScripting;
using UnityEngine;

public class AgentMemoryWorldCell : WorldCell {

	private bool _explored;
	private AgentMemoryWorldCell[] _neighbours;

	public AgentMemoryWorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates,
		WorldCellType worldCellType = WorldCellType.Normal, bool explored = false) : base(cellCoordinates, worldCoordinates, worldCellType) {
		this._neighbours = new AgentMemoryWorldCell[6];
		this._explored = explored;
	}

	public void Explore() {
		_explored = true;
	}
	
	public bool IsExplored() {
		return _explored;
	}
	
	public void AddNeighbour(AgentMemoryWorldCell newNeighbour, int pos){
		_neighbours[pos] = newNeighbour;
	}
	
	public void AddNeighbour(AgentMemoryWorldCell newNeighbour, Direction direction) {
		AddNeighbour(newNeighbour, (int) direction);
	}
	
	public new AgentMemoryWorldCell GetNeighbour(int pos) {
		return _neighbours[pos];
	}
	
	public AgentMemoryWorldCell GetNeighbour(Direction direction) {
		return GetNeighbour((int) direction);
	}


	public new AgentMemoryWorldCell[]  GetNeighbours() {
		return _neighbours;
	}
}