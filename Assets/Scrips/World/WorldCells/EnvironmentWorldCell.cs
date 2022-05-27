using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEditor;
using UnityEngine;


public class EnvironmentWorldCell : WorldCell  {
    private Agent _currentAgent;

    private GameObject _foodObject;

    public EnvironmentWorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates,
        WorldCellType worldCellType = WorldCellType.Normal) : this(cellCoordinates,
        worldCoordinates, new EnvironmentWorldCell[6], WorldCellType.Normal) {}
    
    public EnvironmentWorldCell(Vector3Int cellCoordinates, Vector3 worldCoordinates, EnvironmentWorldCell[] neighbours, WorldCellType worldCellType = WorldCellType.Normal) : base(cellCoordinates, worldCoordinates, neighbours, worldCellType) {
        this._currentAgent = null;
    }
    
    public bool ContainsFood() {
        return _foodObject != null;
    }

    public void SpawnFood(GameObject foodObject, Transform parent) {
        _foodObject = GameObject.Instantiate(foodObject, worldCoordinates, Quaternion.identity, parent);
    }

    public void ConsumeFood() {
        GameObject.Destroy(_foodObject);
    }

    public void Occupy(Agent agent) {
        this._currentAgent = agent;
    }

    public void Unoccupy() {
        this._currentAgent = null;
    }
    
    public bool IsOccupied() {
        return _currentAgent != null;
    }

    public Agent GetAgent() {
        return _currentAgent;
    }
}