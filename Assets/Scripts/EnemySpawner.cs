using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public GameObject enemyPrefab;

    public void SpawnEnemies(List<RectInt> generatedRooms, Vector2Int furthestRoom)
    {
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

    public void RemoveEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }
}
