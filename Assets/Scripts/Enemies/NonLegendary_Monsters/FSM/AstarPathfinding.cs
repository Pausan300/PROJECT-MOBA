using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinding
{
    public LayerMask obstacleLayer;

    public List<AstarNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log("FindPath called");

        AstarNode startNode = new AstarNode(startPos);
        AstarNode targetNode = new AstarNode(targetPos);

        List<AstarNode> openSet = new List<AstarNode> { startNode };
        HashSet<AstarNode> closedSet = new HashSet<AstarNode>();

        while (openSet.Count > 0)
        {
            AstarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position == targetNode.position)
            {
                Debug.Log("Path found");
                return RetracePath(startNode, currentNode);
            }

            foreach (AstarNode neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.Log("No path found");
        return new List<AstarNode>();
    }

    List<AstarNode> RetracePath(AstarNode startNode, AstarNode endNode)
    {
        List<AstarNode> path = new List<AstarNode>();
        AstarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    float GetDistance(AstarNode nodeA, AstarNode nodeB)
    {
        float dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        float dstY = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        float dstZ = Mathf.Abs(nodeA.position.z - nodeB.position.z);

        return dstX + dstY + dstZ;
    }

    List<AstarNode> GetNeighbors(AstarNode node)
    {
        List<AstarNode> neighbors = new List<AstarNode>();

        Vector3[] directions = {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPos = node.position + direction;
            Debug.DrawRay(node.position, direction, Color.red, 1f); // Dibuja el rayo en la escena

            if (!Physics.Raycast(node.position, direction, 1f, obstacleLayer))
            {
                Debug.Log($"No obstacle detected in direction {direction} from position {node.position}");
                neighbors.Add(new AstarNode(neighborPos));
            }
            else
            {
                Debug.Log($"Obstacle detected in direction {direction} from position {node.position}");
            }
        }

        return neighbors;
    }
}