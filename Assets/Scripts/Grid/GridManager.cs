using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridManager 
{
    public static int gridWidth, gridHeight;

    public static GridGraph gridGraph;

    public static float currentCost = 1000;

    public static void CreateGrid(int width,int height)
    {
        gridWidth = width;
        gridHeight = height;

        gridGraph = new GridGraph(gridWidth, gridHeight);
    }
    public static Node GetNode(int x,int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return null;

        return gridGraph.Grid[x, y];
    }
    public static Node GetNodeFromTile(Tile tile)
    {
        return gridGraph.Grid[tile.x, tile.y];
    }
    public static Node GetClosestNodeForShortBus(Transform itemTransform)
    {
        Node node = gridGraph.Grid[0,0];

        foreach (var item in gridGraph.Grid)
        {
            if (node.tileType!=TileType.Empty)
            {
                node = item;
            }
            else if ((Vector3.Distance(item.worldPosition, itemTransform.position) <
                Vector3.Distance(node.worldPosition, itemTransform.position)) &&
                item.tileType==TileType.Empty)
            {
                node = item;
            }
        }

        return node;
    }
    public static Node GetClosestNodeForLongBus(Transform itemTransform,BusDirection direction,Bus bus )
    {
        Node node = gridGraph.Grid[0, 0];

        bool firstChange = false;

        foreach (var item in gridGraph.Grid)
        {

            Node nextNode = GetNextNodeForLongBus(item, direction);

            bool isCurrentTileFree = item.tileType == TileType.Empty ||
                           (item.tileType == TileType.Bus && item.currentBus == bus);

            bool isNextTileFree = IsBoundsInside(item, direction) &&
                                   (nextNode.tileType == TileType.Empty ||
                                   (nextNode.tileType == TileType.Bus && nextNode.currentBus == bus));

            bool isDistanceShort = Vector3.Distance(item.worldPosition, itemTransform.position) <=
                Vector3.Distance(node.worldPosition, itemTransform.position);

            if (!firstChange && isCurrentTileFree && isNextTileFree)
            {
                firstChange = true;
                node = item;
            }
            
            if (isCurrentTileFree && isNextTileFree && isDistanceShort)
            {
                node = item;
            }
        }
        return node;
    }
    public static Node GetNextNodeForLongBus(Node node,BusDirection direction)
    {
        if (!IsBoundsInside(node, direction))
            return null;


        if (direction == BusDirection.Vertical)
            return GetNode((int)node.Position.x,
                                 (int)node.Position.y + 1);
        else
            return GetNode((int)node.Position.x + 1,
                                (int)node.Position.y);
        
    }
    public static bool IsBoundsInside(Node node, BusDirection direction)
    {
        if (direction == BusDirection.Vertical)
        {
            if ((int)node.Position.y + 1 < gridHeight)
                return true;
        }    
        else
            if ((int)node.Position.x + 1 < gridWidth)
                return true;

        return false;
    }
    public static void ClearPath()
    {
        currentCost = 1000;
    }
    public static List<Node> MakePath(Passanger passanger)
    {
        ClearPath();

        var targetList = GetTarget(passanger);

        List<Node> path = new List<Node>();

        if (targetList==null || gridGraph.Grid[gridWidth - 1, gridHeight - 1].tileType != TileType.Empty)
            return null;

        for (int i = 0; i < targetList.Count; i++)
        {
            var a = AStar.Search(gridGraph, gridGraph.Grid[gridWidth-1, gridHeight-1], gridGraph.Grid[(int)targetList[i].Position.x, (int)targetList[i].Position.y]);

            if (a != null /*&& AStar.cost < currentCost*/)
            {
                currentCost = AStar.cost;
               // lastTargetPos = new Vector2((targetList[i].x), 0);

                path = AStar.Search(gridGraph, gridGraph.Grid[gridWidth - 1, gridHeight - 1], gridGraph.Grid[(int)targetList[i].Position.x, (int)targetList[i].Position.y]);
            }
        }

        if (path == null)
        {
            return null;
        }

        return path;

    }
    public static List<Node> MakePath(Bus bus)
    {
        ClearPath();

        List<Node> path = new List<Node>();


        var a = AStar.Search(gridGraph, bus.GetCurrentNode(), gridGraph.Grid[gridWidth - 1, 0],bus);

        if (a != null /*&& AStar.cost < currentCost*/)
        {
            currentCost = AStar.cost;
            // lastTargetPos = new Vector2((targetList[i].x), 0);

            path = AStar.Search(gridGraph, bus.GetCurrentNode(), gridGraph.Grid[gridWidth - 1, 0],bus);
        }


        if (path == null)
        {
            return null;
        }

        return path;

    }
    public static List<Node> CanHaveTargetLastTile(Passanger passanger)
    {
        List<Node> targetList = new List<Node>();

        foreach (var item in GetTarget(passanger))
        {
            if (IsLastTile(item) && item.currentBus.direction==BusDirection.Horizontal)
            {
                targetList.Add(item);
                return targetList;
            }
        }

        return null;
    }
    public static List<Node> GetTarget(Passanger passanger)
    {
        List<Node> targetList = new List<Node>();

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Node node = gridGraph.Grid[j, i];
                if (node.tileType == TileType.Bus &&
                !node.currentBus.IsItemFull() &&
                node.currentBus.color == passanger.color &&
                !node.currentBus.isSelected &&
               !targetList.Contains(node))
                {
                    targetList.Add(node);
                }
            }
        }

        return targetList.OrderBy(c => Vector3.Distance(gridGraph.Grid[gridWidth-1, gridHeight-1].worldPosition, c.worldPosition)).Reverse().ToList();

    }
    public static bool IsLastTile(Node node)
    {
        if (node.Position.x == gridWidth - 1 && node.Position.y == gridHeight - 1)
            return true;

        return false;
    }
}
