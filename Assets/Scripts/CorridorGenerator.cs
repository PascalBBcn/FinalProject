using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator
{
    public static HashSet<Vector2Int> CreateCorridors(List<(Vector2Int, Vector2Int)> roomConnections)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        // Create a corridor for each pairing of closest rooms
        foreach (var connection in roomConnections)
        {
            HashSet<Vector2Int> newCorridor = CreateACorridor(connection.Item1, connection.Item2);
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private static HashSet<Vector2Int> CreateACorridor(Vector2Int room1, Vector2Int room2)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int pos = room1;
        corridor.Add(pos);

        // Find the difference in directions, then add that direction until point reached
        if (Random.value > 0.5f)
        {
            int xDirection = (int)Mathf.Sign(room2.x - room1.x);
            while (pos.x != room2.x)
            {
                pos += new Vector2Int(xDirection, 0);
                corridor.Add(pos);
                corridor.Add(pos + Vector2Int.up);   // above
                corridor.Add(pos + Vector2Int.down); // below
            }
            int yDirection = (int)Mathf.Sign(room2.y - room1.y);
            while (pos.y != room2.y)
            {
                pos += new Vector2Int(0, yDirection);
                corridor.Add(pos);
                corridor.Add(pos + Vector2Int.left);  // left
                corridor.Add(pos + Vector2Int.right); // right
            }
        }
        else
        {
            int yDirection = (int)Mathf.Sign(room2.y - room1.y);
            while (pos.y != room2.y)
            {
                pos += new Vector2Int(0, yDirection);
                corridor.Add(pos);
                corridor.Add(pos + Vector2Int.left);  // left
                corridor.Add(pos + Vector2Int.right); // right
            }
            int xDirection = (int)Mathf.Sign(room2.x - room1.x);
            while (pos.x != room2.x)
            {
                pos += new Vector2Int(xDirection, 0);
                corridor.Add(pos);
                corridor.Add(pos + Vector2Int.up);   // above
                corridor.Add(pos + Vector2Int.down); // below
            }
        }
        return corridor;
    }
    
    
    // A hybrid approach (uses recursion on L/R subtrees along with proximity checks between rooms within them)
    public static List<(Vector2Int, Vector2Int)> GetRoomConnectionPairings(BSPNode node, List<Vector2Int> roomCenterPoints)
    {
        List<(Vector2Int, Vector2Int)> roomConnections = new List<(Vector2Int, Vector2Int)>();

        if (node.IsLeaf()) return roomConnections;

        // Get center points of leftsubtree/rightsubtree leaves
        List<Vector2Int> leftLeaves = new List<Vector2Int>();
        List<Vector2Int> rightLeaves = new List<Vector2Int>();
        GetValidLeafCenters(node.LeftChild, roomCenterPoints, leftLeaves);
        GetValidLeafCenters(node.RightChild, roomCenterPoints, rightLeaves);


        // Connect the closest pair between left and right subtrees
        if (leftLeaves.Count > 0 && rightLeaves.Count > 0)
        {
            // Compares ALL rooms in left subtree vs ALL rooms in right subtree
            (Vector2Int, Vector2Int) closestRooms = FindClosest(leftLeaves, rightLeaves);
            roomConnections.Add(closestRooms);
        }

        // Recursively get the connections of the left/right children
        var leftSubtree = GetRoomConnectionPairings(node.LeftChild, roomCenterPoints);
        foreach (var connection in leftSubtree)
        {
            roomConnections.Add(connection);
        }
        var rightSubtree = GetRoomConnectionPairings(node.RightChild, roomCenterPoints);
        foreach (var connection in rightSubtree)
        {
            roomConnections.Add(connection);
        }
        return roomConnections;
    }

    // Needed because some leaves may be too small (due to xSplit/ySplit variation)
    private  static void GetValidLeafCenters(BSPNode node, List<Vector2Int> roomCenterPoints, List<Vector2Int> results)
    {
        if (node == null) return;

        if (node.IsLeaf())
        {
            Vector2Int center = Vector2Int.FloorToInt(node.Bounds.center);
            // Check if the roomCenters list contains the node's bounds center, if so, add it to results
            // Some of the leaves have bounds far too small, and these will be ignored
            if (roomCenterPoints.Contains(center))
            {
                results.Add(center);
            }
        }
        else
        {
            GetValidLeafCenters(node.LeftChild, roomCenterPoints, results);
            GetValidLeafCenters(node.RightChild, roomCenterPoints, results);
        }
    }


    // Ensures that the shortest (proximity wise) connection is made between two partitions
    private static (Vector2Int, Vector2Int) FindClosest(List<Vector2Int> leftSubtree, List<Vector2Int> rightSubtree)
    {
        (Vector2Int, Vector2Int) closestRooms = (Vector2Int.zero, Vector2Int.zero);
        float minDistance = Mathf.Infinity;
        
        foreach (var validLeafL in leftSubtree)
        {
            foreach (var validLeafR in rightSubtree)
            {
                float distance = Vector2Int.Distance(validLeafL, validLeafR);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestRooms = (validLeafL, validLeafR);
                }
            }
        }
        
        return closestRooms;
    }

}
