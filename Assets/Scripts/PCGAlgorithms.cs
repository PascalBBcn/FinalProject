using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Need not be MonoBehaviour, as this script is not directly attaching to a GameObject,
// this script is simply for implementing PCG algorithms
public static class PCGAlgorithms
{
    // For agent-based approach
    public static Vector2Int GetRandomCardinalDirection()
    {
        List<Vector2Int> directionList = new List<Vector2Int>
        {
            new Vector2Int(0, 1), // UP
            new Vector2Int(0, -1), // DOWN
            new Vector2Int(1, 0), // RIGHT 
            new Vector2Int(-1, 0) // LEFT
        };

        return directionList[Random.Range(0, directionList.Count)];
    }

    public static HashSet<Vector2Int> AgentBasedDig(int numberOfDigs, Vector2Int agentPosition)
    {
        HashSet<Vector2Int> dungeonFloor = new HashSet<Vector2Int>();

        int chanceOfChangingDirection = 5;
        int chanceOfAddingRoom = 5;

        // Place agent at origin and randomize direction
        Vector2Int direction = GetRandomCardinalDirection();
        dungeonFloor.Add(agentPosition);

        while (dungeonFloor.Count < numberOfDigs)
        {
            // Dig along this direction
            agentPosition += direction;
            dungeonFloor.Add(agentPosition);

            // Ensure no single width pathways
            Vector2Int extraTile;
            if (direction.x != 0) extraTile = new Vector2Int(0, 1);
            else extraTile = new Vector2Int(1, 0);
            dungeonFloor.Add(agentPosition + extraTile);

            // Potentially change digging direction
            if (Random.Range(0, 100) < chanceOfChangingDirection)
            {
                direction = GetRandomCardinalDirection(); // Step 7
                chanceOfChangingDirection = 0;
            }
            else chanceOfChangingDirection += 5;

            // Potentially add ROOM
            if (Random.Range(0, 100) < chanceOfAddingRoom)
            {
                // Room dimensions
                int width = Random.Range(3, 7);
                int height = Random.Range(3, 7);

                // Place room around agent position
                for (int x = -width / 2; x <= width / 2; x++)
                {
                    for (int y = -height / 2; y <= height / 2; y++)
                    {
                        Vector2Int tile = agentPosition + new Vector2Int(x, y);
                        dungeonFloor.Add(tile);
                    }
                }
                chanceOfAddingRoom = 0;
            }
            else chanceOfAddingRoom += 5;
        }

        return dungeonFloor;
    }



}
