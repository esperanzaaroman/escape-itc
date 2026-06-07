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
        int cols  = gridManager.grid.GetLength(1);

        for (int x = 0; x < lines; x++)
            for (int y = 0; y < cols; y++)
            {
                Node n = gridManager.grid[x, y];
                if (n != null) { n.gCost = int.MaxValue; n.hCost = 0; n.Parent = null; }
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

            if (++safetyCounter > maxNodes)
            {
                Debug.LogError("[A*] Bucle infinito detectado en RetracePath.");
                return new List<Node>();
            }
        }

        path.Reverse();
        return path;
    }

    /// <param name="profundidadMax">
    /// Cuántas iteraciones hace el for antes de detenerse
    /// </param>
    public List<Node> FindPath(Node startNode, Node targetNode, Queue<Node> historialProfe, int profundidadMax = -1)
    {
        if (startNode == null || targetNode == null || gridManager == null)
            return new List<Node>();

        ResetGridNodes();

        List<Node> openSet      = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        // Límite de iteraciones: si profundidadMax es -1 usamos int.MaxValue
        int limite = (profundidadMax > 0) ? profundidadMax : int.MaxValue;

        for (int iteracion = 0; iteracion < limite && openSet.Count > 0; iteracion++)
        {
            // Seleccionar nodo con menor FCost
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                   (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Objetivo encontrado 
            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            // Expandir vecinos
            foreach (Node vecino in gridManager.GetNeighbours(currentNode))
            {
                if (!vecino.walkable || closedSet.Contains(vecino) ||
                    (historialProfe != null && historialProfe.Contains(vecino)))
                    continue;

                int nuevoCosto = currentNode.gCost + GetDistance(currentNode, vecino);

                if (nuevoCosto < vecino.gCost || !openSet.Contains(vecino))
                {
                    vecino.gCost  = nuevoCosto;
                    vecino.hCost  = GetDistance(vecino, targetNode);
                    vecino.Parent = currentNode;

                    if (!openSet.Contains(vecino))
                        openSet.Add(vecino);
                }
            }
        }

        // Se agotaron las iteraciones — devolver camino al nodo más cercano al objetivo
        if (openSet.Count > 0)
        {
            Node mejorParcial = openSet[0];
            foreach (Node n in openSet)
                if (n.hCost < mejorParcial.hCost)
                    mejorParcial = n;

            return RetracePath(startNode, mejorParcial);
        }

        return new List<Node>();
    }
}