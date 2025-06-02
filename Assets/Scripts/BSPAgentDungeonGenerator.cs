using System.Collections.Generic;
using UnityEngine;

// The main script of the game
// Uses BSP for macro, but fills rooms with Agent-based "digged" rooms
public class BSPAgentDungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonWidth = 70;
    [SerializeField] private int dungeonHeight = 70;
    [SerializeField] private int minRoomWidth = 12;
    [SerializeField] private int minRoomHeight = 12;
    [SerializeField] private int numberOfDigs = 100;


    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private TileRenderer tileRenderer; // Referencing our tileRenderer script

    public void StartGeneration()
    {
        // Ensure tileRenderer exists
        if (tileRenderer == null)
        {
            Debug.LogError("TileRenderer missing!");
            return;
        }
        // "Refresh" game scene each time before repopulating it
        tileRenderer.RemoveTiles();
        CreateRooms();
    }
    private void CreateRooms()
    {
        RectInt dungeonSpace = new RectInt(startPos.x, startPos.y, dungeonWidth, dungeonHeight);
        BSPNode rootNode = PCGAlgorithms.BinarySpacePartitioning(dungeonSpace, minRoomWidth, minRoomHeight);

        int totalLeaves = rootNode.CountLeafNodes();
        Debug.Log($"Number of leaf nodes: {totalLeaves}");

        List<RectInt> rooms = new List<RectInt>();
        rootNode.GetLeafNodes(rooms, minRoomWidth, minRoomHeight);

        // Uses the agent-based approach to create rooms
        HashSet<Vector2Int> dungeonFloor = CreateRandomRooms(rooms);

        // Get center points of all rooms
        List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
        foreach (var room in rooms)
        {
            roomCenterPoints.Add(Vector2Int.FloorToInt(room.center));
        }

        // Generate the positions for corridor placements 
        List<(Vector2Int, Vector2Int)> roomConnectionPairings = CorridorGenerator.GetRoomConnectionPairings(rootNode, roomCenterPoints);
        // Generate corridors
        HashSet<Vector2Int> corridors = CorridorGenerator.CreateCorridors(roomConnectionPairings);
        dungeonFloor.UnionWith(corridors);

        RenderTiles(dungeonFloor);
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<RectInt> rooms)
    {
        HashSet<Vector2Int> dungeonFloor = new HashSet<Vector2Int>();
        foreach (var room in rooms)
        {
            var agentBasedRoom = PCGAlgorithms.AgentBasedDig(numberOfDigs, Vector2Int.FloorToInt(room.center));
            dungeonFloor.UnionWith(agentBasedRoom);
        }
        return dungeonFloor;
    }

    private void RenderTiles(HashSet<Vector2Int> dungeonFloor)
    {
        tileRenderer.SetFloorTiles(dungeonFloor);
        tileRenderer.SetWallTiles(dungeonFloor);
    }
}
