using UnityEngine;

public class Node
{
    public int x;
    public int y;

    public bool walkable;

    public int gCost;
    public int hCost;

    public Vector3 worldPosition;

    public Node Parent;

    public int FCost
    {
        get{return gCost+hCost;}
    }

    public Node(bool walkable, Vector3 worldPosition,int x, int y)
    {
        this.x = x;
        this.y= y;
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        gCost = int.MaxValue; // infinito
        hCost = 0;
        Parent = null;
    }
}
