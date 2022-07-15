using Priority_Queue;
using Scrips.Agent;

public class WorldCellAStar : FastPriorityQueueNode {

	private WorldCell _correspondingWorldCell;
	private WorldCellAStar _previousWorldCell;
	private Direction _incomingDirection;

	private float _distanceFromStart;
	
	public WorldCellAStar(WorldCell correspondingWorldCell, WorldCellAStar previousWorldCell,
		Direction incomingDirection, float distanceFromStart) : base() {
		
		this._correspondingWorldCell = correspondingWorldCell;
		this._previousWorldCell = previousWorldCell;
		this._incomingDirection = incomingDirection;
		this._distanceFromStart = distanceFromStart;
	}

	public void SetPreviousWorldCell(WorldCellAStar previousWorldCell, Direction incomingDirection, float distanceFromStart) {
		this._previousWorldCell = previousWorldCell;
		this._incomingDirection = incomingDirection;
		this._distanceFromStart = distanceFromStart;
	}

	public WorldCell GetWorldCell() {
		return _correspondingWorldCell;
	}

	public WorldCellAStar GetPreviousWorldCell() {
		return _previousWorldCell;
	}

	public Direction GetPreviousDirection() {
		return _incomingDirection;
	}
	
	public float GetDistanceFromStart() {
		return _distanceFromStart;
	}
	
}