using System;
using System.Collections.Generic;
using Scrips.Agent;
using UnityEngine;

public class HexagonGridUtility {
    public static Vector3Int[] GetCoordinatesOfNeighbouringCells(Vector3Int gridCoordinates) {
        Vector3Int[][] neighbourOffsets = new Vector3Int[][] {
            new Vector3Int[] {
                new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(-1,1,0),
                new Vector3Int(-1,0,0), new Vector3Int(-1,-1,0), new Vector3Int(0,-1,0)
            },
            new Vector3Int[] {
                new Vector3Int(1,0,0), new Vector3Int(1,1,0), new Vector3Int(0,1,0),
                new Vector3Int(-1,0,0), new Vector3Int(0,-1,0), new Vector3Int(1,-1,0)
            }
        };

        Vector3Int[] neighbouringCoordinates = new Vector3Int[6];

        // check if row is odd. If thats true the value is 1 otherwise false. This is important for selecting the correct offsets.
        int isOddRow = gridCoordinates.y % 2;

        for(int i = 0; i < 6; i++) {
            neighbouringCoordinates[i] = gridCoordinates + neighbourOffsets[isOddRow][i];
        }

        return neighbouringCoordinates;
    }

    // Return the coordinate that is to a certain direction of an agent
    public static Vector3Int GetCoordinatesOfNeighbouringCell(Vector3Int gridCoordinate, Direction direction) {
        Vector3Int[][] neighbourOffsets = new Vector3Int[][] {
            new Vector3Int[] {
                new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 1, 0),
                new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0)
            },
            new Vector3Int[] {
                new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0),
                new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0)
            }
        };

        return gridCoordinate + neighbourOffsets[Math.Abs(gridCoordinate.y) % 2][(int) direction];
    }
    
    public static List<Vector3Int> GetGridCellCircleCoordinates(Vector3Int center, int radius) {
        List<Vector3Int> circleCoordinates = new List<Vector3Int>();

        for (int r = 1; r <= radius; r++) {
            Vector3Int hex = center + new Vector3Int(r, 0);
            
            foreach(Direction i in new []{Direction.NW, Direction.W, Direction.SW, Direction.SE, Direction.E, Direction.NE}) {
                for (int j = 0; j < r; j++) {
                    circleCoordinates.Add(hex);
                    hex = GetCoordinatesOfNeighbouringCell(hex, i);
                }
            }
        }

        return circleCoordinates;
    }

    // Source: https://answers.unity.com/questions/960064/hexagon-grid-distance.html
    public static double GetHexGridDistance(Vector3Int a, Vector3Int b) {
        int dx = b.x - a.x;     // signed deltas
        int dy = b.y - a.y;
        int x = Mathf.Abs(dx);  // absolute deltas
        int y = Mathf.Abs(dy);
        // special case if we start on an odd row or if we move into negative x direction
        if ((dx < 0)^((a.y&1)==1))
            x = Mathf.Max(0, x - (y + 1) / 2);
        else
            x = Mathf.Max(0, x - (y) / 2);
        return x + y;
    }
    
    // Source: https://www.redblobgames.com/grids/hexagons/#distances
    public static double GetHexGridWidthDistance(Vector3Int a, Vector3Int b) {
        double dcol = Math.Abs(a.y - b.y);
        double drow = Math.Abs(a.x - b.x);
        return drow + Math.Max(0, (dcol - drow) / 2);
    }
    
    // Source: https://www.redblobgames.com/grids/hexagons/#distances
    public static double GetHexGridHeightDistance(Vector3Int a, Vector3Int b) {
        double dcol = Math.Abs(a.y - b.y);
        double drow = Math.Abs(a.x - b.x);
        return dcol + Math.Max(0, (drow - dcol) / 2);
    }
}
