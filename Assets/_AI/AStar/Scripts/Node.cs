﻿using UnityEngine;

/// <summary>
/// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
/// </summary>
public class Node
{
    public Vector2Int position; //Position on the grid
    public Node parent; //Parent Node of this node

    public float FScore { //GScore + HScore
        get { return GScore + HScore; }
    }
    public float GScore; //Current Travelled Distance
    public float HScore; //Distance estimated based on Heuristic

    public Node() { }
    public Node(Vector2Int position, Node parent, int GScore, int HScore)
    {
        this.position = position;
        this.parent = parent;
        this.GScore = GScore;
        this.HScore = HScore;
    }
}
