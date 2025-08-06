using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for enemy movement
public static class Pathfinding
{
    private static HashSet<Vector2Int> dungeonFloor;
    public static void Initialize(HashSet<Vector2Int> floor)
    {
        dungeonFloor = new HashSet<Vector2Int>(floor);
    }
    // A structure to hold the necessary parameters
    public class Cell
    {
        public Vector2Int parent;
        public double f, g, h;
    }
    public static List<Vector2Int> AStar(Vector2Int enemyPos, Vector2Int playerPos)
    {
        if (!dungeonFloor.Contains(enemyPos) || !dungeonFloor.Contains(playerPos)) return new List<Vector2Int>();
        // If player already reached
        if (enemyPos.x == playerPos.x && enemyPos.y == playerPos.y) return new List<Vector2Int> { enemyPos };
        // Unvisited tiles
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();
        // To store parameters for each tile
        Dictionary<Vector2Int, Cell> cellDetails = new Dictionary<Vector2Int, Cell>();

        foreach (var tile in dungeonFloor)
        {
            cellDetails[tile] = new Cell
            {
                f = double.MaxValue,
                g = double.MaxValue,
                h = double.MaxValue,
                parent = new Vector2Int(-1, -1)
            };
        }
        // Initialize starting tile
        cellDetails[enemyPos].f = 0.0;
        cellDetails[enemyPos].g = 0.0;
        cellDetails[enemyPos].h = 0.0;
        cellDetails[enemyPos].parent = enemyPos;
        SortedSet<(double, Vector2Int)> openList =
        new SortedSet<(double, Vector2Int)>(Comparer<(double, Vector2Int)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
        openList.Add((0.0, enemyPos));

        bool playerFound = false;

        while (openList.Count > 0)
        {
            (double f, Vector2Int pos) currentTile = openList.Min;
            openList.Remove(currentTile);
            closedList.Add(currentTile.pos);
            List<Vector2Int> directions = new List<Vector2Int> { // Checking all 8 neighbours
                new Vector2Int(1,0), new Vector2Int(-1,0),
                new Vector2Int(0,1), new Vector2Int(0,-1),
                new Vector2Int(1,1), new Vector2Int(-1,1),
                new Vector2Int(1,-1), new Vector2Int(-1,-1)
            };
            foreach (var dir in directions)
            {
                Vector2Int neighbourPos = currentTile.pos + dir;
                if (!dungeonFloor.Contains(neighbourPos)) continue;
                if (neighbourPos == playerPos) // Check if neighbourPos is playerPos
                {
                    cellDetails[neighbourPos].parent = currentTile.pos;
                    playerFound = true;
                    return TracePath(cellDetails, playerPos);
                }
                if (!closedList.Contains(neighbourPos))// If neighbourPos is already visited, ignore it
                {
                    double cost = (dir.x == 0 || dir.y == 0) ? 1.0 : 1.414;
                    double gNew = cellDetails[currentTile.pos].g + cost;
                    double hNew = CalculateHValue(neighbourPos, playerPos);
                    double fNew = gNew + hNew;
                    if (cellDetails[neighbourPos].f == double.MaxValue || cellDetails[neighbourPos].f > fNew)
                    {
                        openList.Add((fNew, neighbourPos));
                        cellDetails[neighbourPos].f = fNew;
                        cellDetails[neighbourPos].g = gNew;
                        cellDetails[neighbourPos].h = hNew;
                        cellDetails[neighbourPos].parent = currentTile.pos;
                    }
                }
            }
        }
        if (!playerFound) return new List<Vector2Int>();
        return new List<Vector2Int>();
    }


    public static double CalculateHValue(Vector2Int pos, Vector2Int playerPos)
    {
        return Vector2Int.Distance(pos, playerPos);  // Euclidean distance (best for any directional movement)
    }
    public static List<Vector2Int> TracePath(Dictionary<Vector2Int, Cell> cellDetails, Vector2Int playerPos)
    {
        List<Vector2Int> pathData = new List<Vector2Int>();
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        while (cellDetails[playerPos].parent != playerPos)
        {
            path.Push(playerPos);
            Vector2Int tempPos = cellDetails[playerPos].parent;
            playerPos = tempPos;
        }
        path.Push(playerPos);
        while (path.Count > 0)
        {
            Vector2Int p = path.Peek();
            pathData.Add(path.Pop());
        }
        return pathData;
    }
}
