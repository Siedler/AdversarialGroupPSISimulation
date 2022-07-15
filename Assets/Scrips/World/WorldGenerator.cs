using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour {

    public string pathToWorld;

    private Grid _grid;
    private Tilemap[] _tileMaps;
    public Tile[] tiles = new Tile[4];

    // TODO Remove testGameObject
    public GameObject testGameObject;
    
    private Environment _environment;
    
    public void Initiate() {
        _environment = GetComponent<Environment>();
        
        _grid = this.transform.GetChild(0).GetComponent<Grid>();

        _tileMaps = new Tilemap[4];
        
        _tileMaps[0] = _grid.transform.Find("Ocean").GetComponent<Tilemap>();
        _tileMaps[1] = _grid.transform.Find("Forest").GetComponent<Tilemap>();
        _tileMaps[2] = _grid.transform.Find("Mountain").GetComponent<Tilemap>();
        _tileMaps[3] = _grid.transform.Find("Grass").GetComponent<Tilemap>();

        WorldStructure worldStructure = ReadWorld();
        RenderWorld(worldStructure);
    }

    private WorldStructure ReadWorld() {
        if (!File.Exists(pathToWorld)) return null;
        
        StreamReader reader = new StreamReader(pathToWorld); 

        string jsonString = reader.ReadToEnd();
        
        reader.Close();

        return new WorldStructure(jsonString, _grid);
    }

    // Start is called before the first frame update
    private void RenderWorld(WorldStructure worldStructure) {
        if(worldStructure == null) return;

        Dictionary<Vector3Int, EnvironmentWorldCell> environment = new Dictionary<Vector3Int, EnvironmentWorldCell>();
        List<EnvironmentWorldCell> spawnTeam1 = new List<EnvironmentWorldCell>();
        List<EnvironmentWorldCell> spawnTeam2 = new List<EnvironmentWorldCell>();
        List<EnvironmentWorldCell> spawnFood = new List<EnvironmentWorldCell>();

        // Clear all tiles so that the map can be generated clearly
        foreach(Tilemap t in _tileMaps) {
            t.ClearAllTiles();
        }

        int pos = 0;
        for(int h = worldStructure.height-1; h >= 0; h--) {
            for(int w = 0; w < worldStructure.width; w++) {
                int elem = worldStructure.world[pos];

                Vector3Int cellCoordinates = new Vector3Int(w, h, 0);
                _tileMaps[elem].SetTile(cellCoordinates, tiles[elem]);

                // If the given Tile is a walkable grass tile
                if(elem == 3) {
                    Vector3 worldCoordinates = _grid.GetCellCenterWorld(cellCoordinates);

                    WorldCellType worldCellType = WorldCellType.Normal;
                    if (worldStructure.spawnTeam1CellCoordinates.Contains(cellCoordinates)) {
                        worldCellType = WorldCellType.SpawnTeam1;
                    } else if (worldStructure.spawnTeam2CellCoordinates.Contains(cellCoordinates)) {
                        worldCellType = WorldCellType.SpawnTeam2;
                    } else if (worldStructure.spawnFoodCoordinates.Contains(cellCoordinates)) {
                        worldCellType = WorldCellType.SpawnFood;
                    }
                    
                    // Create a new WorldCell
                    EnvironmentWorldCell newEnvironmentWorldCell = new EnvironmentWorldCell(cellCoordinates, worldCoordinates, worldCellType);

                    Vector3Int[] neighbouringCellCoordinates = HexagonGridUtility.GetCoordinatesOfNeighbouringCells(cellCoordinates);
                    for(int i = 0; i < 6; i++) {
                        // Neighbour already exists! Connecting them
                        if(!environment.ContainsKey(neighbouringCellCoordinates[i])) continue;
                        
                        EnvironmentWorldCell neighbouringCell = environment[neighbouringCellCoordinates[i]];

                        newEnvironmentWorldCell.AddNeighbour(neighbouringCell, i);
                        neighbouringCell.AddNeighbour(newEnvironmentWorldCell, (i+3)%6);
                    }

                    environment.Add(cellCoordinates, newEnvironmentWorldCell);
                    // TODO do more elegantly
                    if (worldStructure.spawnTeam1CellCoordinates.Contains(cellCoordinates)) {
                        spawnTeam1.Add(newEnvironmentWorldCell);
                    } else if (worldStructure.spawnTeam2CellCoordinates.Contains(cellCoordinates)) {
                        spawnTeam2.Add(newEnvironmentWorldCell);
                    } else if (worldStructure.spawnFoodCoordinates.Contains(cellCoordinates)) {
                        spawnFood.Add(newEnvironmentWorldCell);
                    }
                }

                pos++;
            }
        }

        // Give the world map to the script that is handling the environment
        _environment.SetEnvironmentMap(environment, spawnTeam1.ToArray(), spawnTeam2.ToArray(), spawnFood.ToArray());

        Debug.Log("World Rendered");
    }

}
