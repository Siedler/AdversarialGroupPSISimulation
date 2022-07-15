using UnityEngine;

public class FoodController : MonoBehaviour {
	private Environment _environment;

	private EnvironmentWorldCell[] _spawnPoints;

	public GameObject foodPrefab;

	[Range(0f, 1f)]
	public float chanceToRespawnFood = 0.5f;
	
	public void InitiateFood(EnvironmentWorldCell[] spawnPoints) {
		_environment = GameObject.Find("World").GetComponent<Environment>();
		_spawnPoints = spawnPoints;
	}
	
	public void Tick(int timeStep) {
		foreach (EnvironmentWorldCell foodSpawn in _spawnPoints) {
			if(foodSpawn.ContainsFood()) continue;

			float r = Random.Range(0f, 1f);
			if(r <= chanceToRespawnFood) SpawnFood(foodSpawn);
		}
	}

	private void SpawnFood(EnvironmentWorldCell foodSpawn) {
		foodSpawn.SpawnFood(foodPrefab, this.transform);
	}
	
}
