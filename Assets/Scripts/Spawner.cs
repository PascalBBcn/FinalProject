using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    private GameObject playerInstance;
    public GameObject exitPrefab;
    private GameObject exitInstance;

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

    public void SpawnInstances(List<RectInt> generatedRooms, BSPDungeonGenerator generator, Vector2Int furthestRoom)
    {
        Debug.Log(generatedRooms[0].center);
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, generatedRooms[0].center, Quaternion.identity);
            Camera.main.GetComponent<FollowPlayerCamera>().player = playerInstance.transform;
        }
        else
        {
            // Move existing player to new level's start position
            playerInstance.transform.position = generatedRooms[0].center;
        }
        exitInstance = Instantiate(exitPrefab, (Vector3Int)furthestRoom, Quaternion.identity);
        exitInstance.GetComponent<LevelExit>().SetGenerator(generator);

        for (int i = 1; i < generatedRooms.Count - 1; i++)
        {
            // Skip the exit room
            if (furthestRoom.x >= generatedRooms[i].x && furthestRoom.x < generatedRooms[i].xMax &&
        furthestRoom.y >= generatedRooms[i].y && furthestRoom.y < generatedRooms[i].yMax) continue;

            // int r = Random.Range(1, 5);
            int r = 1;
            while (r > 0)
            {
                // Random position
                int x = Random.Range(generatedRooms[i].x + 3, generatedRooms[i].xMax - 3);
                int y = Random.Range(generatedRooms[i].y + 3, generatedRooms[i].yMax - 3);
                Vector2 spawnPos = new Vector2(x, y);

                GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                spawnedEnemies.Add(enemyInstance);
                r--;
            }
            
        }
    }
    
    // For room locking system
    public bool EnemiesAreAlive(RectInt room)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy == null) continue;
            Vector2Int enemyPos = new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.y));
            if (room.Contains(enemyPos)) return true;
        }
        return false;
    }

    public void RemoveInstances()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        Destroy(exitInstance);
        spawnedEnemies.Clear();
    }
}
