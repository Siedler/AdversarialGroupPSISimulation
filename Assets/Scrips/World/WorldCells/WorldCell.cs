using UnityEngine;

// TODO Look if WorldCellType is needed
public enum WorldCellType {
	Normal,
	SpawnTeam1,
	SpawnTeam2,
	SpawnFood,
}

public class WorldCell {
	public Vector3Int cellCoordinates;
	public Vector3 worldCoordinates;

	private WorldCell[] _neighbours;

	private WorldCellType _worldCellType;

	public WorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates,
		WorldCellType worldCellType = WorldCellType.Normal) : this(cellCoordinates,
		worldCoordinates, new WorldCell[6], WorldCellType.Normal) {}
	
	public WorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates, WorldCell[] neighbours, WorldCellType worldCellType = WorldCellType.Normal){
		this.cellCoordinates = cellCoordinates;
		this.worldCoordinates = worldCoordinates;

		this._worldCellType = worldCellType;
        
		this._neighbours = neighbours;
	}
	
	public void SetWorldCellType(WorldCellType worldCellType) {
		this._worldCellType = worldCellType;
	}

	public WorldCellType GetWorldCellType() {
		return _worldCellType;
	}
	
	public void AddNeighbour(WorldCell newNeighbour, int pos) {
		_neighbours[pos] = newNeighbour;
	}

	public WorldCell GetNeighbour(int pos) {
		return _neighbours[pos];
	}

	public WorldCell[] GetNeighbours() {
		return _neighbours;
	}
}