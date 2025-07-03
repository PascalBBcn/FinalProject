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

    public WeaponDB weaponDB;
    public GameObject weaponPrefab;
    private GameObject weaponInstance;

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
        RoomData bossRoom = null;
        for (int i = 1; i < roomData.Count; i++)
        {
            if (roomData[i].roomType == RoomType.Chest) chestRoom = roomData[i];
            else if (roomData[i].roomType == RoomType.Boss) bossRoom = roomData[i];
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
            Camera.main.GetComponent<FollowPlayerCamera>().player = playerInstance.transform;
        }
        else
        {
            // Move existing player to new level's start position
            playerInstance.transform.position = startRoom.bounds.center;
        }

        // LEVEL EXIT SPAWNING
        exitInstance = Instantiate(exitPrefab, bossRoom.bounds.center, Quaternion.identity);
        exitInstance.GetComponent<LevelExit>().SetGenerator(generator);

        enemySpawner.SpawnEnemies(roomData);
    }

    

    public void RemoveInstances()
    {
        enemySpawner.RemoveEnemies();
        Destroy(weaponInstance);
        Destroy(exitInstance);
    }
}
