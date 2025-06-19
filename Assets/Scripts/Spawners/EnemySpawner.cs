using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public GameObject enemyPrefab;

    public void SpawnEnemies(List<RoomData> roomData)
    {
        for (int i = 1; i < roomData.Count - 1; i++)
        {
            if (roomData[i].roomType != RoomType.Boss && roomData[i].roomType != RoomType.Chest)
            {
                // int r = Random.Range(1, 5);
                int r = 1;
                while (r > 0)
                {
                    // Random position
                    int x = Random.Range(roomData[i].bounds.x + 3, roomData[i].bounds.xMax - 3);
                    int y = Random.Range(roomData[i].bounds.y + 3, roomData[i].bounds.yMax - 3);
                    Vector2 spawnPos = new Vector2(x, y);

                    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    enemyInstance.GetComponent<EnemyMovement>().roomBounds = roomData[i].bounds;
                    spawnedEnemies.Add(enemyInstance);
                    r--;
                }
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
