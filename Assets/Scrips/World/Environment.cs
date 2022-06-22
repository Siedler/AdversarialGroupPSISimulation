using System.Collections.Generic;
using System.IO;
using Scrips.Agent;
using UnityEngine;

public class Environment : MonoBehaviour {

    private WorldGenerator _worldGenerator;
    
    // Create a Hashtable that stores the walkable tiles
    private Dictionary<Vector3Int, EnvironmentWorldCell> _environmentMap;
    private EnvironmentWorldCell[] _spawnTeam1;
    private EnvironmentWorldCell[] _spawnTeam2;
    private EnvironmentWorldCell[] _spawnFood;
    
    private Grid _grid;

    private AgentController _team1Controller;
    private AgentController _team2Controller;
    private FoodController _foodController;

    public int _agentSeed;
    public int _gameSeed;

    // Start is called before the first frame update
    public void Initialize() {
        _grid = this.transform.GetChild(0).GetComponent<Grid>();

        _worldGenerator = GetComponent<WorldGenerator>();

        _team1Controller = GameObject.Find("Team1").GetComponent<AgentController>();
        _team2Controller = GameObject.Find("Team2").GetComponent<AgentController>();
        _foodController = GameObject.Find("Food Controller").GetComponent<FoodController>();
        
        _worldGenerator.Initiate();

        Random.InitState(_agentSeed);
        _team1Controller.InitiateAgents(_spawnTeam1, 0, Direction.E);
        _team2Controller.InitiateAgents(_spawnTeam2, 1, Direction.W);
        Random.InitState(_gameSeed);
        
        _foodController.InitiateFood(_spawnFood);
    }

    // Execute actions of one time-step
    public void Tick() {
        Debug.Log("Tick");
        _foodController.Tick();
        
        _team1Controller.Tick();
        _team2Controller.Tick();
    }

    public List<EnvironmentWorldCell> SenseWorld(EnvironmentWorldCell currentEnvironmentWorldCell, Direction facingDirection) {
        if (facingDirection == Direction.Unspecified)
            throw new InvalidDataException("Got Unspecified as the agents direction!");

        List<Vector3Int> visibleWorldCellCoordinates = HexagonGridUtility.GetGridCellCircleCoordinates(currentEnvironmentWorldCell.cellCoordinates, radius: SimulationSettings.AgentViewDistance);

        List<EnvironmentWorldCell> visibleWorldCells = new List<EnvironmentWorldCell>();
        
        foreach(Vector3Int visibleCoordinate in visibleWorldCellCoordinates) {
            // Add the visible world cell if it's contained in the environment
            // TODO: Remove world cells which view is obstructed, i.e. by an obstacle
            
            EnvironmentWorldCell visibleEnvironmentWorldCell = (_environmentMap.ContainsKey(visibleCoordinate)
                ? _environmentMap[visibleCoordinate]
                : null);
            visibleWorldCells.Add(visibleEnvironmentWorldCell);
        }
        
        return visibleWorldCells;
    }

    public Vector3Int GetCellCoordinateFromWorldCoordinate(Vector3 worldCoordinate) {
        return _grid.WorldToCell(worldCoordinate);
    }

    public Vector3 GetWorldCoordinateFromCellCoordinate(Vector3Int cellCoordinate) {
        return _grid.CellToWorld(cellCoordinate);
    }
    public EnvironmentWorldCell GetWorldCellByCoordinates(Vector3Int worldCellCoordinates) {
        return _environmentMap.ContainsKey(worldCellCoordinates) ? _environmentMap[worldCellCoordinates] : null;
    }

    public EnvironmentWorldCell GetWorldCellByCoordinates(Vector3 worldCoordinate) {
        return GetWorldCellByCoordinates(GetCellCoordinateFromWorldCoordinate(worldCoordinate));
    }

    public bool DoesCellWithCoordinateExist(Vector3Int coordinate) {
        return _environmentMap.ContainsKey(coordinate);
    }

    public void SetEnvironmentMap(Dictionary<Vector3Int, EnvironmentWorldCell> environmentMap, EnvironmentWorldCell[] spawnTeam1, EnvironmentWorldCell[] spawnTeam2, EnvironmentWorldCell[] spawnFood) {
        this._environmentMap = environmentMap;

        this._spawnTeam1 = spawnTeam1;
        this._spawnTeam2 = spawnTeam2;
        this._spawnFood = spawnFood;
    }

    public Dictionary<Vector3Int, EnvironmentWorldCell> GetEnvironmentMap() {
        return _environmentMap;
    }
    
    // TODO DEBUG
    public Grid GetGrid() {
        return _grid;
    }
}
