using System.Collections.Generic;
using UnityEngine;


// The main script of the game
public class BSPDungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonWidth = 80;
    [SerializeField] private int dungeonHeight = 80;
    [SerializeField] private int minRoomWidth = 13;
    [SerializeField] private int minRoomHeight = 13;
    private int padding = 1; // For padding between rooms
    [SerializeField] private int numberOfDigs = 200;


    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private TileRenderer tileRenderer;

    [SerializeField] private Spawner spawner;
    // Need access to enemySpawner just for room-locking-system
    [SerializeField] private EnemySpawner enemySpawner;


    // Give read-access only to other scripts
    public HashSet<Vector2Int> corridors { get; private set; }
    public HashSet<Vector2Int> dungeonFloor { get; private set; }
    public List<RectInt> rooms { get; private set; } = new List<RectInt>();

    HashSet<Vector2Int> organicBossRoom = new HashSet<Vector2Int>();
    private Vector2Int bossRoomPos = new Vector2Int(300, 300);

    public void StartGame()
    {
        GameSession.instance.ResetGame();
        StartGeneration();
    }
    public void StartGeneration()
    {
        GameSession.instance.IncreaseFloor();
        GameSession.instance.bossHealthBar.fillAmount = 1f;
        GameSession.instance.bossHealthBarContainer.SetActive(false);

        Debug.Log(GameSession.instance.currentFloor);
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
        GameSession.instance.playerHealthBarContainer.SetActive(true);
        Pathfinding.Initialize(dungeonFloor);
    }

    private void CreateRooms()
    {
        RectInt dungeonSpace = new RectInt(startPos.x, startPos.y, dungeonWidth, dungeonHeight);
        BSPNode rootNode = PCGAlgorithms.BinarySpacePartitioning(dungeonSpace, minRoomWidth, minRoomHeight);

        rooms = new List<RectInt>();
        rootNode.GetLeafNodes(rooms, minRoomWidth, minRoomHeight);

        dungeonFloor = CreateRectangularRooms(rooms);

        // Get center points of all rooms (for corridor creation)
        List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
        foreach (var room in rooms)
        {
            Debug.Log(room.width * room.height);
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

        // ADD DOORS
        foreach (var room in rooms)
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

            foreach (var pos in edgePositions)
            {
                if (corridors.Contains(pos))
                {
                    tileRenderer.SetSingleUnlockedDoor(pos);
                }
            }
        }

        // Agent-based boss room (far away)
        organicBossRoom = GenerateBossRoom();
        dungeonFloor.UnionWith(organicBossRoom);
        RectInt organicBossRoomBounds = GetBoundsFromOrganicRoom(organicBossRoom);
        RoomData organicBossRoomData = new RoomData(organicBossRoomBounds, RoomType.OrganicBoss);

        RenderTiles(dungeonFloor);
        // tileRenderer.HideRooms(rooms);

        // For "tagging" rooms via different roomTypes
        List<RoomData> roomData = RoomAssigner.AssignRooms(rooms, furthestRoom);
        roomData.Add(organicBossRoomData);

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
                if (corridors.Contains(pos)) tileRenderer.SetSingleLockedDoor(pos);
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

    public HashSet<Vector2Int> GenerateBossRoom()
    {
        HashSet<Vector2Int> dungeonFloor = new HashSet<Vector2Int>();
        var agentBasedRoom = PCGAlgorithms.AgentBasedDig(numberOfDigs, bossRoomPos);
        dungeonFloor.UnionWith(agentBasedRoom);
        return dungeonFloor;
    }

    private RectInt GetBoundsFromOrganicRoom(HashSet<Vector2Int> tiles)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (var tile in tiles)
        {
            if (tile.x < minX) minX = tile.x;
            if (tile.y < minY) minY = tile.y;
            if (tile.x > maxX) maxX = tile.x;
            if (tile.y > maxY) maxY = tile.y;
        }
        return new RectInt(minX, minY, maxX - minX, maxY - minY);

    }

    public void TeleportToBossRoom(GameObject player)
    {
        
        Vector2Int furthestPosFromBoss = Vector2Int.zero;
        float maxDist = float.MinValue;
        foreach (var pos in organicBossRoom)
        {
            float dist = Vector2.Distance(pos, bossRoomPos);
            if (dist > maxDist)
            {
                maxDist = dist;
                furthestPosFromBoss = pos;
            }

        }
        if (!organicBossRoom.Contains(furthestPosFromBoss)) Debug.Log("PLAYER SPAWN ERROR");
        player.transform.position = new Vector3(furthestPosFromBoss.x +0.5f, furthestPosFromBoss.y+0.5f, 0);
    }
}


