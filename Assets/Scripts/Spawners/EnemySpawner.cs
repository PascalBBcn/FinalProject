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
            int enemyCount = GetEnemyCount(roomData[i]);
            if (roomData[i].roomType != RoomType.Boss && roomData[i].roomType != RoomType.Chest)
            {
                while (enemyCount > 0)
                {
                    // Random position
                    int x = Random.Range(roomData[i].bounds.x + 3, roomData[i].bounds.xMax - 3);
                    int y = Random.Range(roomData[i].bounds.y + 3, roomData[i].bounds.yMax - 3);
                    Vector2 spawnPos = new Vector2(x, y);

                    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    enemyInstance.GetComponent<EnemyMovement>().roomBounds = roomData[i].bounds;
                    spawnedEnemies.Add(enemyInstance);
                    enemyCount--;
                }
            }


        }
    }
    
    public int GetEnemyCount(RoomData roomData)
    {
        //if too small, cap the number of enemies which can spawn in that room 
        bool roomIsTooSmall = (roomData.bounds.width * roomData.bounds.height) < 250;
        int enemyCount = 0;
        int floor = GameSession.instance.currentFloor;
        float difficulty = GameSession.instance.difficultyMultiplier;

        switch (floor)
        {
            case 1:
                if (Random.value < 0.7f) enemyCount = Random.Range(1, 3); // 70%
                else if (Random.value < 0.8f) enemyCount = Random.Range(4, 7); // 10%
                else enemyCount = 0; // 20%
                break;
            case 2:
                if (Random.value < 0.7f) enemyCount = Random.Range(3, 6);
                else if (Random.value < 0.8f) enemyCount = Random.Range(7, 10);
                else enemyCount = Random.Range(2, 4);

                if (roomIsTooSmall && enemyCount > 5) enemyCount = 5;
                break;
            case 3:
                if (Random.value < 0.7f) enemyCount = Random.Range(7, 11);
                else if (Random.value < 0.8f) enemyCount = Random.Range(12, 17);
                else enemyCount = Random.Range(5, 7);

                if (roomIsTooSmall && enemyCount > 6) enemyCount = 6;
                break;
            case 4:
                if (Random.value < 0.7f) enemyCount = Random.Range(7, 13);
                else if (Random.value < 0.8f) enemyCount = Random.Range(14, 18);
                else enemyCount = Random.Range(5, 7);

                if (roomIsTooSmall && enemyCount > 6) enemyCount = 6;
                break;
            default:
                if (Random.value < 0.7f) enemyCount = Random.Range(7, 14);
                else if (Random.value < 0.8f) enemyCount = Random.Range(15, 18);
                else enemyCount = Random.Range(5, 7);

                if (roomIsTooSmall && enemyCount > 6) enemyCount = 6;
                break;
            
        }

        return Mathf.Clamp(enemyCount, 0, 18);
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
