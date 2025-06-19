using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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

    [SerializeField] private Spawner spawner;
    // Need access to enemySpawner just for room-locking-system
    [SerializeField] private EnemySpawner enemySpawner;


    // Give read-access only to other scripts
    public HashSet<Vector2Int> corridors { get; private set; }
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
        spawner.RemoveInstances();
        // "Refresh" game scene each time before repopulating it
        tileRenderer.RemoveTiles();

        CreateRooms();
        Pathfinding.Initialize(dungeonFloor);
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
        corridors = CorridorGenerator.CreateCorridors(roomConnectionPairings);
        dungeonFloor.UnionWith(corridors);

        // Furthest room is used for Boss/Exit room
        // Using thin corridors (to avoid redundant processes from 3-wide corridor)
        Vector2Int furthestRoom = spawner.GetFurthestRoomFromStart(roomCenterPoints, CorridorGenerator.thinCorridors);

        RenderTiles(dungeonFloor);

        // For "tagging" rooms via different roomTypes
        List<RoomData> roomData = RoomAssigner.AssignRooms(rooms, furthestRoom);

        // spawner.SpawnInstances(rooms, this, furthestRoom);
        spawner.SpawnInstances(roomData, this);

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
    
    // Locks the room if enemies within room are alive, unlocks otherwise
    public void LockRoom(RectInt room)
    {
        List<Vector2Int> edgePositions = new List<Vector2Int>();

        for (int x = room.xMin + 1; x < room.xMax - 1; x++)
        {
            edgePositions.Add(new Vector2Int(x, room.yMin));
            edgePositions.Add(new Vector2Int(x, room.yMax - 1));
        }

        for (int y = room.yMin + 1; y < room.yMax - 1; y++)
        {
            edgePositions.Add(new Vector2Int(room.xMin, y));
            edgePositions.Add(new Vector2Int(room.xMax - 1, y));
        }
        // If enemies still alive, keep the room locked
        if (enemySpawner.EnemiesAreAlive(room))
        {
            foreach (var pos in edgePositions)
            {
                if (corridors.Contains(pos)) tileRenderer.SetSingleDoor(pos);
            }
        }
        // "Unlock" room (by removing those wall tiles) once all enemies in room defeated
        else
        {
            foreach (var pos in edgePositions)
            {
                if (corridors.Contains(pos)) tileRenderer.RemoveTile(pos);
            }
        }
    }
}
