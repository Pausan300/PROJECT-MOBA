using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector3 Position { get; set; }
    public PathNode Parent { get; set; }
    public float GCost { get; set; }
    public float HCost { get; set; }
    public float FCost => GCost + HCost;

    public PathNode(Vector3 position)
    {
        Position = position;
    }
}

public class AStarPathfinding
{
    private PriorityQueue<PathNode> openList;
    private HashSet<PathNode> closedList;
    private Vector3[] directions = new Vector3[]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    public List<PathNode> GeneratedNodes { get; private set; } = new List<PathNode>();

    public List<Vector3> FindPath(Vector3 start, Vector3 target, float nodeSize, LayerMask obstacleMask, int maxNodesPerFrame = 100)
    {
        openList = new PriorityQueue<PathNode>();
        closedList = new HashSet<PathNode>();
        GeneratedNodes.Clear();

        PathNode startNode = new PathNode(start);
        PathNode targetNode = new PathNode(target);

        openList.Enqueue(startNode, startNode.FCost);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.Dequeue();
            GeneratedNodes.Add(currentNode);

            if (currentNode.Position == targetNode.Position)
            {
                Debug.Log("Path found");
                return RetracePath(startNode, currentNode);
            }

            closedList.Add(currentNode);

            foreach (Vector3 direction in directions)
            {
                Vector3 neighborPosition = currentNode.Position + direction * nodeSize;
                if (Physics.CheckSphere(neighborPosition, nodeSize / 2, obstacleMask))
                {
                    Debug.Log("Obstacle detected at " + neighborPosition);
                    continue;
                }

                PathNode neighborNode = new PathNode(neighborPosition);
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                float newGCost = currentNode.GCost + Vector3.Distance(currentNode.Position, neighborNode.Position);
                if (newGCost < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newGCost;
                    neighborNode.HCost = Vector3.Distance(neighborNode.Position, targetNode.Position);
                    neighborNode.Parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Enqueue(neighborNode, neighborNode.FCost);
                    }
                }
            }

            if (closedList.Count >= maxNodesPerFrame)
            {
                Debug.Log("Max nodes per frame reached");
                return null; // Limitar la cantidad de nodos explorados por frame
            }
        }

        Debug.Log("No path found");
        return null;
    }

    private List<Vector3> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool Contains(T item)
    {
        foreach (var element in elements)
        {
            if (EqualityComparer<T>.Default.Equals(element.Key, item))
            {
                return true;
            }
        }
        return false;
    }
}