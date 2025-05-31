using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Responsible for placing tiles on the tilemap, depending on positions given
public class TileRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private TileBase floorTile;

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
    
    public void RemoveTiles()
    {
        floorTilemap.ClearAllTiles();
    }

    
}
