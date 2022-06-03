using Scrips.Agent;
using Unity.VisualScripting;
using UnityEngine;

public class AgentMemoryWorldCell : WorldCell {

	private bool _explored;
	private readonly AgentMemoryWorldCell[] _neighbours;

	private double[] _needSatisfactionAssociations;

	public AgentMemoryWorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates,
		WorldCellType worldCellType = WorldCellType.Normal, bool explored = false) : base(cellCoordinates, worldCoordinates, worldCellType) {
		this._neighbours = new AgentMemoryWorldCell[6];
		this._explored = explored;

		this._needSatisfactionAssociations = new double[5];
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


	public new AgentMemoryWorldCell[] GetNeighbours() {
		return _neighbours;
	}

	public void UpdateNeedSatisfactionAssociations(
		double painAvoidanceSatisfaction,
		double energySatisfaction,
		double affiliationSatisfaction,
		double certaintySatisfaction,
		double competenceSatisfaction) {

		double[] satisfactionValues = new double[] {
			painAvoidanceSatisfaction,
			energySatisfaction,
			affiliationSatisfaction,
			certaintySatisfaction,
			competenceSatisfaction,
		};

		for (int i = 0; i < _needSatisfactionAssociations.Length; i++) {
			_needSatisfactionAssociations[i] += satisfactionValues[i];

			if (_needSatisfactionAssociations[i] > 1) _needSatisfactionAssociations[i] = 1;
			else if (_needSatisfactionAssociations[i] < -1) _needSatisfactionAssociations[i] = -1;
		}
	}

	public double[] GetNeedSatisfactionAssociations() {
		return _needSatisfactionAssociations;
	}

	// Apply forget rate to all need satisfaction associations
	// Input:
	// forgetRatePositiveNegative: double[2][num_of_needs] of forget values [..., [forget_p, forget_n], ...] where 0 <= forget_x <= 1
	public void Forget(double[][] forgetRatePositiveNegative) {
		// Update every value depending on if its a negative or positive association
		for (int i = 0; i < _needSatisfactionAssociations.Length; i++) {
			_needSatisfactionAssociations[i] *= forgetRatePositiveNegative[_needSatisfactionAssociations[i] >= 0 ? 0 : 1][i];
		}
	}
}