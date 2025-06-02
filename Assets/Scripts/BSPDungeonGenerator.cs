using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


// The main script of the game
public class BSPDungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonWidth = 80;
    [SerializeField] private int dungeonHeight = 80;
    [SerializeField] private int minRoomWidth = 13;
    [SerializeField] private int minRoomHeight = 13;
    private int padding = 2; // For padding between rooms

    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private TileRenderer tileRenderer; 

    // Give read-access only to other scripts
    public HashSet<Vector2Int> dungeonFloor { get; private set; }
    public List<RectInt> rooms { get; private set; } = new List<RectInt>();


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
        Debug.Log($"Num of leaf nodes: {totalLeaves}");

        rooms = new List<RectInt>();
        rootNode.GetLeafNodes(rooms, minRoomWidth, minRoomHeight);

        dungeonFloor = CreateRectangularRooms(rooms);

        // Get center points of all rooms (for corridor creation)
        List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
        foreach (var room in rooms)
        {
            roomCenterPoints.Add(Vector2Int.FloorToInt(room.center));
        }

        // Generate the positions for corridor placements
        // These are pairs of connections
        List<(Vector2Int, Vector2Int)> roomConnectionPairings = CorridorGenerator.GetRoomConnectionPairings(rootNode, roomCenterPoints);
        // Generate corridors
        HashSet<Vector2Int> corridors = CorridorGenerator.CreateCorridors(roomConnectionPairings);
        dungeonFloor.UnionWith(corridors);


        RenderTiles(dungeonFloor);
    }

    private HashSet<Vector2Int> CreateRectangularRooms(List<RectInt> rooms)
    {
        HashSet<Vector2Int> dungeonFloor = new HashSet<Vector2Int>();
        foreach (var room in rooms)
        {
            // Iterate through the room bounds, including the padding around it
            for (int col = padding; col < room.width - padding; col++)
            {
                for (int row = padding; row < room.height - padding; row++)
                {
                    // Find the position for the room
                    Vector2Int position = new Vector2Int(room.x + col, room.y + row);
                    dungeonFloor.Add(position);
                }
            }
        }
        return dungeonFloor;
    }

    private void RenderTiles(HashSet<Vector2Int> dungeonFloor)
    {
        tileRenderer.SetFloorTiles(dungeonFloor); // Render based on dungeonFloor positions
        tileRenderer.SetWallTiles(dungeonFloor);
    }
}
