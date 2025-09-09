using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class Spawner : MonoBehaviour
{
    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Spawners")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WeaponSpawner weaponSpawner;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private GameObject teleportPrefab;
    [SerializeField] private GameObject bossPrefab1;
    [SerializeField] private GameObject bossPrefab2;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject healthPickupPrefab;

    // INSTANCES
    private GameObject playerInstance;
    private GameObject exitInstance;
    private GameObject teleportInstance;
    private GameObject bossInstance;
    private GameObject weaponInstance;
    private GameObject healthPickupInstance;

    private RoomData organicBossRoom; // Cache so SpawnLevelExit method can use it
    private Vector3 bossSpawnPos = new Vector3(300, 300, 0);
    private Vector3 offScreen = new Vector3(20000, 20000, 0); // Initially spawn the exit instance totally offscreen

    // Event subscription: boss death
    private void OnEnable()
    {
        EnemyStats.OnBossDeath += SpawnLevelExit;
    }
    private void OnDisable()
    {
        EnemyStats.OnBossDeath -= SpawnLevelExit;
    }

    // Walks through the corridors
    // Corridors does contain the center points as well
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

    public void SpawnInstances(List<RoomData> roomData, DungeonGenerator generator)
    {
        int floor = GameSession.instance.currentFloor;
        GameObject currentFloorBoss = bossPrefab1;

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
            playerInstance = Instantiate(playerPrefab, startRoom.bounds.center, Quaternion.identity);
            virtualCamera.Follow = playerInstance.transform;
        }
        else playerInstance.transform.position = startRoom.bounds.center; // Move existing player to new level's start position

        // LEVEL TELEPORT/EXIT/PICKUP SPAWNING
        teleportInstance = Instantiate(teleportPrefab, bossRoomData.bounds.center, Quaternion.identity);
        teleportInstance.GetComponent<LevelTeleport>().SetGenerator(generator);
        exitInstance = Instantiate(exitPrefab, offScreen, Quaternion.identity);
        exitInstance.GetComponent<LevelExit>().SetGenerator(generator);
        healthPickupInstance = Instantiate(healthPickupPrefab, offScreen, Quaternion.identity);

        // ENEMY/BOSS SPAWNING
        enemySpawner.SpawnEnemies(roomData);
        // Spawn different boss depending on floor
        switch (floor)
        {
            case 1:
                currentFloorBoss = bossPrefab1;
                break;
            case 2:
                currentFloorBoss = bossPrefab2;
                break;
            case 3:
                currentFloorBoss = bossPrefab1;
                break;
            default:
                currentFloorBoss = bossPrefab1;
                break;
        }
        // Spawn ranged enemies with boss on floor3
        if (floor == 3)
        {
            Vector3 off = new Vector3(1f, 0f, 0f);
            Vector3 pos = (Vector3)bossSpawnPos + off;

            GameObject enemyRanged1 = Instantiate(enemySpawner.enemyRangedPrefab, pos, Quaternion.identity);
            GameObject enemyRanged2 = Instantiate(enemySpawner.enemyRangedPrefab, pos, Quaternion.identity);
            enemyRanged1.GetComponent<EnemyMovement>().roomBounds = organicBossRoom.bounds;
            enemyRanged2.GetComponent<EnemyMovement>().roomBounds = organicBossRoom.bounds;
        }
        bossInstance = Instantiate(currentFloorBoss, bossSpawnPos, Quaternion.identity);
        bossInstance.GetComponent<EnemyMovement>().roomBounds = organicBossRoom.bounds;
    }

    private void SpawnLevelExit()
    {
        // YOU WON THE GAME
        if (GameSession.instance.currentFloor == 3)
        {
            GameSession.instance.winMenu.SetActive(true);
            return;
        }
        
        exitInstance.transform.position = bossSpawnPos;
        // Create offset for health pickup spawn pos
        Vector3 off = new Vector3(1, 0f, 0f);
        Vector3 pos = (Vector3)bossSpawnPos + off;
        healthPickupInstance.transform.position = pos;
    }

    public void RemoveInstances()
    {
        enemySpawner.RemoveEnemies();

        if (BossSelfMultiply.activeInstances != null)
        {
            foreach (var child in BossSelfMultiply.activeInstances.ToList())  // .ToList() to avoid modifying while iterating
            {
                if (child != null) GameObject.Destroy(child);
            }
            BossSelfMultiply.activeInstances.Clear();
        }
        
        Destroy(bossInstance);
        Destroy(weaponInstance);
        Destroy(exitInstance);
        Destroy(teleportInstance);
    }
}
