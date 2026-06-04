using UnityEngine;
using System.Collections.Generic;

public class PathFinding
{
    public GridManager gridManager;

    private int GetDistance(Node a, Node b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private void ResetGridNodes()
    {
        if (gridManager == null || gridManager.grid == null) return;

        int lines = gridManager.grid.GetLength(0);
        int cols = gridManager.grid.GetLength(1);

        for (int x = 0; x < lines; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Node n = gridManager.grid[x, y];
                if (n != null)
                {
                    n.gCost = int.MaxValue; 
                    n.hCost = 0;
                    n.Parent = null;
                }
            }
        }
    }

    public List<Node> RetracePath(Node inicio, Node fin)
    {
        List<Node> path = new List<Node>();
        Node current = fin;

        int safetyCounter = 0;
        int maxNodes = gridManager.grid.GetLength(0) * gridManager.grid.GetLength(1);

        while (current != inicio && current != null)
        {
            path.Add(current);
            current = current.Parent;

            safetyCounter++;
            if (safetyCounter > maxNodes)
            {
                Debug.LogError("[A*] Bucle infinito detectado en RetracePath.");
                return new List<Node>();
            }
        }

        path.Reverse();
        return path;
    }

    public List<Node> FindPath(Node startNode, Node targetNode, Queue<Node> historialProfe)
    {
        if (startNode == null || targetNode == null || gridManager == null) 
            return new List<Node>();

        ResetGridNodes();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || 
                   (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node vecino in gridManager.GetNeighbours(currentNode))
            {
                if (!vecino.walkable || closedSet.Contains(vecino) || (historialProfe != null && historialProfe.Contains(vecino)))
                {
                    continue;
                }

                int nuevoCostoALVecino = currentNode.gCost + GetDistance(currentNode, vecino);

                if (nuevoCostoALVecino < vecino.gCost || !openSet.Contains(vecino))
                {
                    vecino.gCost = nuevoCostoALVecino;
                    vecino.hCost = GetDistance(vecino, targetNode);
                    vecino.Parent = currentNode;

                    if (!openSet.Contains(vecino))
                    {
                        openSet.Add(vecino);
                    }
                }
            }
        }

        return new List<Node>(); 
    }
}