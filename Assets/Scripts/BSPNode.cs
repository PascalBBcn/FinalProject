using System.Collections.Generic;
using UnityEngine;

// Need not be MonoBehaviour, as this script is not directly attaching to a GameObject,
// this script is simply for creating tree data structures
public class BSPNode
{
    // Properties for our node object
    public RectInt Bounds;  
    public BSPNode LeftChild;    
    public BSPNode RightChild;

    // Constructor called on creation of new node
    public BSPNode(RectInt node)
    {
        // Assign the properties
        this.Bounds = node;

        this.LeftChild = null;
        this.RightChild = null;
    }

    // Helper to check if node has children or not (is a leaf node)
    public bool IsLeaf()
    {
        // EIther 0 children, or node MUST have 2 children as this is a BINARY TREE
        return this.LeftChild == null && this.RightChild == null;
    }

    // Helper to get all leaf nodes, using recursion
    public void GetLeafNodes(List<RectInt> leafNodes, int minWidth, int minHeight)
    {
        if (this.IsLeaf())
        {
            // Add leaf node only if it meets the minimum size requirements
            if (this.Bounds.width >= minWidth && this.Bounds.height >= minHeight)
            {
                leafNodes.Add(this.Bounds);
            }
        }
        else
        {
            // If node is not a leaf, use recursion on its children
            if (this.LeftChild != null)
            {
                this.LeftChild.GetLeafNodes(leafNodes, minWidth, minHeight);
            }
            if (this.RightChild != null)
            {
                this.RightChild.GetLeafNodes(leafNodes, minWidth, minHeight);
            }
        }
    }

    // Function for debugging purposes only
    public void PrintTree(string direction = "")
    {
        string nodePosition = string.IsNullOrEmpty(direction) ? "root" : direction;
        Debug.Log($"Node ({nodePosition}): Bounds = {Bounds}, IsLeaf = {IsLeaf()}");

        if (LeftChild != null) LeftChild.PrintTree(direction + "l");
        
        if (RightChild != null) RightChild.PrintTree(direction + "r");
    }

    // Function for debugging purposes only
    public int CountLeafNodes()
    {
        if (this.IsLeaf()) return 1;

        int count = 0;

        if (LeftChild != null) count += LeftChild.CountLeafNodes();

        if (RightChild != null) count += RightChild.CountLeafNodes();

        return count;
    }


}
