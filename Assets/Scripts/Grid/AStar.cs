using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class AStar
{

    public static float cost;

    public static List<Node> Search(GridGraph graph, Node start, Node goal)
    {
        Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();
        Dictionary<Node, float> cost_so_far = new Dictionary<Node, float>();

        List<Node> path = new List<Node>();

        SimplePriorityQueue<Node> frontier = new SimplePriorityQueue<Node>();
        frontier.Enqueue(start, 0);

        came_from.Add(start, start);
        cost_so_far.Add(start, 0);

        Node current = new Node(0, 0);
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal) break; // Early exit

            foreach (Node next in graph.Neighbours(current,goal))
            {
                float new_cost = cost_so_far[current] + graph.Cost(next);
                if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                {
                    cost_so_far[next] = new_cost;
                    came_from[next] = current;
                    float priority = new_cost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    next.Priority = new_cost;
                }
            }
        }

        while (current != start)
        {
            path.Add(current);
            current = came_from[current];
        }
        path.Reverse();

        if (path.Count<=0 || path[path.Count-1].Position!=goal.Position)
            return null;

        return path;
    }
    public static List<Node> Search(GridGraph graph, Node start, Node goal, Bus bus)
    {
        Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();
        Dictionary<Node, float> cost_so_far = new Dictionary<Node, float>();

        List<Node> path = new List<Node>();

        SimplePriorityQueue<Node> frontier = new SimplePriorityQueue<Node>();
        frontier.Enqueue(start, 0);

        came_from.Add(start, start);
        cost_so_far.Add(start, 0);

        Node current = new Node(0, 0);
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal) break; // Early exit

            foreach (Node next in graph.Neighbours(current, goal,bus))
            {
                float new_cost = cost_so_far[current] + graph.Cost(next);
                if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                {
                    cost_so_far[next] = new_cost;
                    came_from[next] = current;
                    float priority = new_cost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    next.Priority = new_cost;
                }
            }
        }

        while (current != start)
        {
            path.Add(current);
            current = came_from[current];
        }
        path.Reverse();

        if (path.Count <= 0 || path[path.Count - 1].Position != goal.Position)
            return null;

        return path;
    }

    public static float Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.Position.x - b.Position.x) + Mathf.Abs(a.Position.y - b.Position.y);
    }
}