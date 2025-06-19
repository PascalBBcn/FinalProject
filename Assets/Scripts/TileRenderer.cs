using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Responsible for placing tiles on the tilemap, depending on positions given
public class TileRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap doorUnlockedTilemap;
    [SerializeField] private Tilemap doorLockedTilemap;

    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase doorUnlockedTile;
    [SerializeField] private TileBase doorLockedTile;

    // Flexible as it can be used to set any sort of tile
    private void SetTiles(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            SetSingleTile(tilemap, tile, position);
        }
    }

    private void SetSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePos = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePos, tile);
    }

    public void SetFloorTiles(HashSet<Vector2Int> floorPositions)
    {
        SetTiles(floorPositions, floorTilemap, floorTile);
    }

    public void SetWallTiles(HashSet<Vector2Int> floorPositions)
    {
        var cardinalDirections = new List<Vector2Int>
        {
            new Vector2Int(0, 1),   // UP
            new Vector2Int(0, -1),  // DOWN
            new Vector2Int(1, 0),   // RIGHT
            new Vector2Int(-1, 0)   // LEFT
        };

        var wallPositions = FindWallPositions(floorPositions, cardinalDirections);
        foreach (var position in wallPositions)
        {
            SetSingleWall(position);
        }
    }

    public void SetSingleWall(Vector2Int position)
    {
        SetSingleTile(wallTilemap, wallTile, position);
    }

    public void SetSingleUnlockedDoor(Vector2Int position)
    {
        SetSingleTile(doorUnlockedTilemap, doorUnlockedTile, position);
    }
    public void SetSingleLockedDoor(Vector2Int position)
    {
        SetSingleTile(doorLockedTilemap, doorLockedTile, position);
    }


    private static HashSet<Vector2Int> FindWallPositions(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                // Find all positions that are not the floor tiles but are near our floor tiles
                if (floorPositions.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }

    public void RemoveTile(Vector2Int position)
    {
        var tilePos = doorLockedTilemap.WorldToCell((Vector3Int)position);
        // wallTilemap.SetTile(tilePos, null);
        doorLockedTilemap.SetTile(tilePos, null);
    }

    public void RemoveTiles()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorUnlockedTilemap.ClearAllTiles();
        doorLockedTilemap.ClearAllTiles();
    }

    
}
