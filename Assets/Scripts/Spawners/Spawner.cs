using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public WeaponSpawner weaponSpawner;

    public GameObject playerPrefab;
    private GameObject playerInstance;
    public GameObject exitPrefab;
    private GameObject exitInstance;
    public GameObject teleportPrefab;
    private GameObject teleportInstance;
    public GameObject bossPrefab;
    private GameObject bossInstance;

    public WeaponDB weaponDB;
    public GameObject weaponPrefab;
    private GameObject weaponInstance;

    // Cache so SpawnLevelExit can use it
    private RoomData organicBossRoom;
    Vector3 bossSpawnPos = new Vector3(100, 100, 0);
    
    private void OnEnable()
    {
        EnemyStats.OnBossDeath += SpawnLevelExit;
    }
    private void OnDisable()
    {
        EnemyStats.OnBossDeath -= SpawnLevelExit;
    }

    // Walks through the corridors
    // Corridors does indeed contain the center points as well
    public Vector2Int GetFurthestRoomFromStart(List<Vector2Int> centers, HashSet<Vector2Int> corridors)
    {
        var cardinalDirections = new List<Vector2Int>
        {
            new Vector2Int(0, 1),   // UP
            new Vector2Int(0, -1),  // DOWN
            new Vector2Int(1, 0),   // RIGHT
            new Vector2Int(-1, 0)   // LEFT
        };

        Vector2Int start = centers[0];
        Queue<(Vector2Int, int)> visitedQueue = new Queue<(Vector2Int, int)>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        visitedQueue.Enqueue((start, 0));
        distances[start] = 0;

        while (visitedQueue.Count > 0)
        {
            var (current, dist) = visitedQueue.Dequeue();

            // Check neighbouring points
            foreach (var direction in cardinalDirections)
            {
                Vector2Int neighbour = current + direction;
                if (corridors.Contains(neighbour) && !distances.ContainsKey(neighbour))
                {
                    distances[neighbour] = dist + 1;
                    visitedQueue.Enqueue((neighbour, dist + 1));
                }
            }
        }
        Vector2Int maxKey = Vector2Int.zero;
        int maxValue = 0;
        foreach (var pair in distances)
        {
            if (pair.Value > maxValue)
            {
                maxValue = pair.Value;
                maxKey = pair.Key;
            }
        }

        return maxKey;
    }

    public void SpawnInstances(List<RoomData> roomData, BSPDungeonGenerator generator)
    {
        RoomData startRoom = roomData[0];
        RoomData chestRoom = null;
        organicBossRoom = null;
        RoomData bossRoomData = null;
        for (int i = 1; i < roomData.Count; i++)
        {
            if (roomData[i].roomType == RoomType.Chest) chestRoom = roomData[i];
            else if (roomData[i].roomType == RoomType.Boss) bossRoomData = roomData[i];
            else if (roomData[i].roomType == RoomType.OrganicBoss) organicBossRoom = roomData[i];
        }

        // WEAPON SPAWNING
        WeaponData randomWeapon = weaponSpawner.GetWeapon();
        weaponInstance = Instantiate(weaponPrefab, chestRoom.bounds.center, Quaternion.identity);
        WeaponPickup pickup = weaponInstance.GetComponent<WeaponPickup>();
        pickup.InitializeWeapon(randomWeapon);

        // PLAYER SPAWNING
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, bossRoomData.bounds.center, Quaternion.identity);
            Camera.main.GetComponent<FollowPlayerCamera>().player = playerInstance.transform;
        }
        else
        {
            // Move existing player to new level's start position
            playerInstance.transform.position = bossRoomData.bounds.center;
        }

        // LEVEL EXIT SPAWNING

        teleportInstance = Instantiate(teleportPrefab, bossRoomData.bounds.center, Quaternion.identity);
        teleportInstance.GetComponent<LevelTeleport>().SetGenerator(generator);

        Vector3 offScreen = new Vector3(20000, 20000, 0); 
        exitInstance = Instantiate(exitPrefab, offScreen, Quaternion.identity);
        exitInstance.GetComponent<LevelExit>().SetGenerator(generator);

        
        enemySpawner.SpawnEnemies(roomData);
        bossInstance = Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);
        bossInstance.GetComponent<EnemyMovement>().roomBounds = organicBossRoom.bounds;
    }   

    private void SpawnLevelExit()
    {
        exitInstance.transform.position = bossSpawnPos;
    }

    public void RemoveInstances()
    {
        enemySpawner.RemoveEnemies();
        Destroy(bossInstance);
        Destroy(weaponInstance);
        Destroy(exitInstance);
        Destroy(teleportInstance);
    }
}
