using System.Collections.Generic;
using UnityEngine;

// The main high-level script of the game
public class AgentDungeonGenerator : MonoBehaviour
{
    [SerializeField] private int numberOfDigs = 200;
    [SerializeField] private TileRenderer tileRenderer;
    Vector2Int agentPosition = Vector2Int.zero;

    // Stores the entire dungeon
    private HashSet<Vector2Int> dungeonFloor = new HashSet<Vector2Int>();

    public void StartGeneration()
    {
        if (tileRenderer == null)
        {
            Debug.LogError("TileRenderer missing!");
            return;
        }
        // "Refresh" game scene each time before repopulating it
        tileRenderer.RemoveTiles(); 
        dungeonFloor.Clear();

        dungeonFloor = PCGAlgorithms.AgentBasedDig(numberOfDigs, agentPosition);
        
        RenderTiles(dungeonFloor);
    }

    private void RenderTiles(HashSet<Vector2Int> dungeonFloor)
    {
        // Render based on dungeonFloor positions
        tileRenderer.SetFloorTiles(dungeonFloor);
        tileRenderer.SetWallTiles(dungeonFloor);
    }
}
