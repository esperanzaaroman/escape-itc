using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public LayerMask wallMask;
    public Vector2 gridWorldSize = new Vector2(9, 13);
    public float nodeRadius = 0.5f;

    int gridSizeX;
    float nodeDiameter;
    int gridSizeY;

    public Node[,] grid;
    public int[,] mapa;

    void Awake()
    {
        InicializarGrid();
    }

    // Centralizamos la inicialización para poder usarla en Awake o en Gizmos
    void InicializarGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CrearGrid();
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX =
            (worldPosition.x - transform.position.x + gridWorldSize.x / 2f)
            / gridWorldSize.x;

        float percentY =
            (worldPosition.z - transform.position.z + gridWorldSize.y / 2f)
            / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Clamp explícito para no salirnos del array por errores de punto flotante.
        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSizeY * percentY), 0, gridSizeY - 1);

        return grid[x, y];
    }
    public List<Node> GetNeighbours(Node nodo)
    {
        List<Node> vecinos = new List<Node>();
        int[,] direcciones =
        {
            {0,1}, // Arriba
            {1,0}, // Derecha
            {0,-1},// Abajo
            {-1,0} // Izquierda
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = nodo.x + direcciones[i, 0];
            int ny = nodo.y + direcciones[i, 1];

            if (nx >= 0 && nx < gridSizeX && ny >= 0 && ny < gridSizeY)
            {
                vecinos.Add(grid[nx, ny]);
            }
        }
        return vecinos;
    }

    void CrearGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // Se resta exactamente la mitad del mundo para centrar el grid en el pivote del objeto.
        // No se modifica la Y para que el CheckSphere quede alineado con la altura del laberinto.
        Vector3 worldBottomLeft = new Vector3(
            transform.position.x - gridWorldSize.x / 2f,
            transform.position.y,
            transform.position.z - gridWorldSize.y / 2f
        );

        // Radio de detección ligeramente menor que nodeRadius para no marcar
        // como muro nodos que apenas rozan la geometría en esquinas.
        float checkRadius = nodeRadius * 0.85f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft 
                    + Vector3.right   * (x * nodeDiameter + nodeRadius) 
                    + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !Physics.CheckSphere(worldPoint, checkRadius, wallMask);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node nodo in grid)
            {
                Gizmos.color = nodo.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(
                    nodo.worldPosition,
                    Vector3.one * (nodeDiameter - 0.1f)
                );
            }
        }
    }
}