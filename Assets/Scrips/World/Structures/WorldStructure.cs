using System.Collections.Generic;
using UnityEngine;

public class WorldStructure {

	private SerializableWorldStructure _worldStructure;

	public int width;
	public int height;

	public int[] world;

	public HashSet<Vector3Int> spawnTeam1CellCoordinates;
	public HashSet<Vector3Int> spawnTeam2CellCoordinates;
	public HashSet<Vector3Int> spawnFoodCoordinates;

	public WorldStructure(string jsonString, Grid grid) {
		_worldStructure = JsonUtility.FromJson<SerializableWorldStructure>(jsonString);

		width = _worldStructure.width;
		height = _worldStructure.height;
		world = _worldStructure.world;
		
		//spawnTeam1CellCoordinates = new Vector3Int[_worldStructure.spawn_team1.Length / 2];
		spawnTeam1CellCoordinates = new HashSet<Vector3Int>();
		spawnTeam2CellCoordinates = new HashSet<Vector3Int>();
		spawnFoodCoordinates = new HashSet<Vector3Int>();
		
		for(int i = 0; i < (_worldStructure.spawn_team1.Length/2); i++) {
			spawnTeam1CellCoordinates.Add(grid.WorldToCell(new Vector3(_worldStructure.spawn_team1[2 * i],
				_worldStructure.spawn_team1[(2 * i) + 1], 0)));
		}

		for(int i = 0; i < (_worldStructure.spawn_team2.Length/2); i++) {
			spawnTeam2CellCoordinates.Add(grid.WorldToCell(new Vector3(_worldStructure.spawn_team2[2 * i],
				_worldStructure.spawn_team2[(2 * i) + 1], 0)));
		}

		for(int i = 0; i < (_worldStructure.spawn_food.Length/2); i++) {
			spawnFoodCoordinates.Add(grid.WorldToCell(new Vector3(_worldStructure.spawn_food[2 * i],
				_worldStructure.spawn_food[(2 * i) + 1], 0)));
		}
	}
	
	public int[] GetWorld() {
		return world;
	}

}
