using System;
using System.Collections.Generic;
using UnityEngine;


// The main script of the game
public class BSPDungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonWidth = 70;
    [SerializeField] private int dungeonHeight = 70;
    [SerializeField] private int minRoomWidth = 12;
    [SerializeField] private int minRoomHeight = 12;
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
