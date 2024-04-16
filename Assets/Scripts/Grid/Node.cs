using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2 Position;
    public Vector3 worldPosition;

    public float Priority;

    public GameObject tileObject;

    public TileType tileType;
    public Bus currentBus;
    

    public Node(int x, int y)
    {
        Position = new Vector2(x, y);
    }

    public void SetCurrentBus(Bus item)
    {
        if (item==null)
        {
            currentBus = null;
            tileType = TileType.Empty;
            return;
        }

        tileType = TileType.Bus;
        currentBus = item;
    }

    public int CompareTo(Node other)
    {
        if (this.Priority < other.Priority) return -1;
        else if (this.Priority > other.Priority) return 1;
        else return 0;
    }
}
