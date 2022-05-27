using System;
using System.Collections;
using System.Collections.Generic;
using Scrips.Agent;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TestHexagon : MonoBehaviour {
    
    private Grid _grid;
    private Tilemap _tilemap;
    
    public Tile normalTile;
    public Tile markedTile;
    public Tile centerTile;

    [Range(1,9)]
    public int radius = 1;
    
    // Start is called before the first frame update
    void Start() {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _tilemap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
    }

    private void Awake() {
        Start();
    }

    private List<Vector3Int> FullCircle(Vector3Int center, int radius) {
        List<Vector3Int> circleCoordinates = new List<Vector3Int>();

        for (int r = 1; r <= radius; r++) {
            Vector3Int hex = center + new Vector3Int(r, 0);
            
            foreach(Direction i in new Direction[] {Direction.NW, Direction.W, Direction.SW, Direction.SE, Direction.E, Direction.NE}) {
                for (int j = 0; j < r; j++) {
                    circleCoordinates.Add(hex);
                    hex = HexagonGridUtility.GetCoordinatesOfNeighbouringCell(hex, i);
                }
            }
        }

        return circleCoordinates;
    }

    private List<Vector3Int> CircleRing(Vector3Int center, int radius) {
        List<Vector3Int> ringCoordinates = new List<Vector3Int>();

        Vector3Int hex = center + new Vector3Int(radius, 0);
        
        foreach(Direction i in new Direction[] {Direction.NW, Direction.W, Direction.SW, Direction.SE, Direction.E, Direction.NE}) {
            for (int j = 0; j < radius; j++) {
                ringCoordinates.Add(hex);
                hex = HexagonGridUtility.GetCoordinatesOfNeighbouringCell(hex, i);
            }
        }

        return ringCoordinates;
    }

    private List<Vector3Int> FullCircle2(Vector3Int center, int radius) {
        List<Vector3Int> ringCoordinates = new List<Vector3Int>();

        for (int i = 1; i <= radius; i++) {
            List<Vector3Int> res = CircleRing(center, i);
            foreach (Vector3Int cor in res) {
                ringCoordinates.Add(cor);
            }
        }

        return ringCoordinates;
    }

    // Update is called once per frame
    void Update() {
        for (int x = 0; x < 20; x++) {
            for (int y = 0; y < 20; y++) {
                Vector3Int coordinate = new Vector3Int(x, y, 0);
                _tilemap.SetTile(coordinate, normalTile);
            }
        }

        Vector3Int baseCoordinate = new Vector3Int(10,10,0);
        _tilemap.SetTile(baseCoordinate, centerTile);

        List<Vector3Int> testList = FullCircle(baseCoordinate, radius);
        foreach (Vector3Int t in testList) {
            _tilemap.SetTile(t, markedTile);
        }
    }
}
