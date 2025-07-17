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
        int chanceOfAddingRoom = 20;

        // Place an intial room around agent position
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                dungeonFloor.Add(agentPosition + new Vector2Int(x, y));
            }
        }


        // Place agent at origin and randomize direction
        Vector2Int direction = GetRandomCardinalDirection();
        dungeonFloor.Add(agentPosition);

        while (dungeonFloor.Count < numberOfDigs)
        {
            // Dig along this direction
            agentPosition += direction;
            dungeonFloor.Add(agentPosition);

            // Ensure no single width pathways
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    dungeonFloor.Add(agentPosition + new Vector2Int(x, y));
                }
            }

            // Potentially change digging direction
            if (Random.Range(0, 100) < chanceOfChangingDirection)
            {
                direction = GetRandomCardinalDirection(); // Step 7
                chanceOfChangingDirection = 5;
            }
            else chanceOfChangingDirection += 5;

            // Potentially add ROOM
            if (Random.Range(0, 100) < chanceOfAddingRoom)
            {
                // Room dimensions
                int width = Random.Range(3, 5);
                int height = Random.Range(3, 5);

                // Place room around agent position
                for (int x = -width / 2; x <= width / 2; x++)
                {
                    for (int y = -height / 2; y <= height / 2; y++)
                    {
                        Vector2Int tile = agentPosition + new Vector2Int(x, y);
                        dungeonFloor.Add(tile);
                    }
                }
                chanceOfAddingRoom = 10;
            }
            else chanceOfAddingRoom += 5;
        }

        return dungeonFloor;
    }

    public static BSPNode BinarySpacePartitioning(RectInt dungeonSpace, int minRoomWidth, int minRoomHeight)
    {
        BSPNode root = new BSPNode(dungeonSpace);
        RecursiveSplit(root, minRoomWidth, minRoomHeight);
        return root;
    }

    private static void RecursiveSplit(BSPNode node, int minRoomWidth, int minRoomHeight)
    {
        if (node.Bounds.width >= minRoomWidth && node.Bounds.height >= minRoomHeight)
        {
            bool nodeWasSplit = false;

            // The larger the multiplier, the stricter the requirement for splitting (so larger rooms)
            const float minSplitMultiplier = 2.5f;

            // Randomly attempt to split vertically or horizontally first
            if (Random.value < 0.5f)
            {
                // VERTICAL
                if (node.Bounds.width >= minRoomWidth * minSplitMultiplier)
                {
                    SplitVertically(node, minRoomWidth);
                    nodeWasSplit = true;
                }
                // HORIZONTAL
                else if (node.Bounds.height >= minRoomHeight * minSplitMultiplier - 0.25f)
                {
                    SplitHorizontally(node, minRoomHeight);
                    nodeWasSplit = true;
                }    
            }
            else
            {
                // HORIZONTAL
                if (node.Bounds.height >= minRoomHeight * minSplitMultiplier)
                {
                    SplitHorizontally(node, minRoomHeight);
                    nodeWasSplit = true;
                }
                // VERTICAL
                else if (node.Bounds.width >= minRoomWidth * minSplitMultiplier - 0.25f)
                {
                    SplitVertically(node, minRoomWidth);
                    nodeWasSplit = true;
                }
            }

            // If split occurred, perform recursion on resulting children
            if (nodeWasSplit)
            {
                if (node.LeftChild != null) RecursiveSplit(node.LeftChild, minRoomWidth, minRoomHeight);
                if (node.RightChild != null) RecursiveSplit(node.RightChild, minRoomWidth, minRoomHeight);
            }
        }
    }

    private static void SplitVertically(BSPNode node, int minRoomWidth)
    {
        if (node.Bounds.width <= minRoomWidth/2) return;
        int xSplit = Random.Range(minRoomWidth/2, node.Bounds.width); 

        // VALUE BELOW MAKES IT LOOK TOO PREDICTABLE (but makes only valid leaves!)
        // if (node.Bounds.width <= minRoomWidth-1) return; // Too narrow to split
        // int xSplit = Random.Range(minRoomWidth, node.Bounds.width - minRoomWidth); // Ensure both rooms are large enough


        // Create new rooms with resulting split amount included
        RectInt room1Rect = new RectInt(node.Bounds.x, node.Bounds.y, xSplit, node.Bounds.height);
        RectInt room2Rect = new RectInt(node.Bounds.x + xSplit, node.Bounds.y, node.Bounds.width - xSplit, node.Bounds.height);

        // Create child nodes in tree
        BSPNode room1Node = new BSPNode(room1Rect);
        BSPNode room2Node = new BSPNode(room2Rect);
        node.LeftChild = room1Node;
        node.RightChild = room2Node;
    }

    private static void SplitHorizontally(BSPNode node, int minRoomHeight)
    {
        if (node.Bounds.height <= minRoomHeight/2) return;
        int ySplit = Random.Range(minRoomHeight/2, node.Bounds.height); // Inclusive range

        // VALUE BELOW MAKES IT LOOK TOO PREDICTABLE (but makes only valid leaves!)
        // if (node.Bounds.height <= minRoomHeight-1) return; // Too narrow to split
        // int ySplit = Random.Range(minRoomHeight, node.Bounds.height - minRoomHeight); // Ensure both rooms are large enough

        // Create new rooms with resulting split amount included
        RectInt room1Rect = new RectInt(node.Bounds.x, node.Bounds.y, node.Bounds.width, ySplit);
        RectInt room2Rect = new RectInt(node.Bounds.x, node.Bounds.y + ySplit, node.Bounds.width, node.Bounds.height - ySplit);

        // Create child nodes in tree
        BSPNode room1Node = new BSPNode(room1Rect);
        BSPNode room2Node = new BSPNode(room2Rect);
        node.LeftChild = room1Node;  // Top child
        node.RightChild = room2Node; // Bottom child
    }  



}
