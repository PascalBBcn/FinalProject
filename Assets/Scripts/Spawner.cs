using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public EnemySpawner enemySpawner;

    public GameObject playerPrefab;
    private GameObject playerInstance;
    public GameObject exitPrefab;
    private GameObject exitInstance;

    // public GameObject gunSmgPrefab;
    // private GameObject gunSmgInstance;
    // public GameObject gunShotgunPrefab;
    // private GameObject gunShotgunInstance;

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

        // gunSmgInstance = Instantiate(gunSmgPrefab, generatedRooms[1].center, Quaternion.identity);
        // gunShotgunInstance = Instantiate(gunShotgunPrefab, generatedRooms[2].center, Quaternion.identity);

        enemySpawner.SpawnEnemies(generatedRooms, furthestRoom);
    }

    public void RemoveInstances()
    {
        enemySpawner.RemoveEnemies();
        // Destroy(gunShotgunInstance);
        // Destroy(gunSmgInstance);

        Destroy(exitInstance);
    }
}
