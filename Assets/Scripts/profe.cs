using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class profe : MonoBehaviour
{
    public coin objetivo;
    public float speed = 20f;
    public GridManager gridManager;
    public PathFinding PathFinding;
    
    private Node currentNode;
    private Node targetNode;
    private List<Node> currentPath = new List<Node>();
    private bool estaMoviendose = false;

    private Queue<Node> historialNodos = new Queue<Node>();
    public int capacidadMemoria = 12; // Cuántos nodos hacia atrás recordará el profe

    void Start()
    {   
        PathFinding = new PathFinding();
        PathFinding.gridManager = this.gridManager;
        
        StartCoroutine(IniciarConRetraso());
    }

    IEnumerator IniciarConRetraso()
    {
        yield return null;
        yield return null;

        if (gridManager == null || gridManager.grid == null) yield break;

        currentNode = gridManager.NodeFromWorldPoint(transform.position);
        if (currentNode == null) yield break;

        Debug.Log("Profe inicializado en nodo: " + currentNode.x + "," + currentNode.y);
    }

    void Update()
    {
        if (gridManager == null || gridManager.grid == null || objetivo == null) return;

        Node nodoActualReal = gridManager.NodeFromWorldPoint(transform.position);
        if (nodoActualReal != null && nodoActualReal != currentNode)
        {
            currentNode = nodoActualReal;
            
            // Cada vez que el profe pisa un nodo nuevo de verdad, lo guarda en su memoria
            ActualizarHistorialMemoria(currentNode);
        }

        Node nodoMoneda = objetivo.getCurrentNode();

        if (nodoMoneda != null && nodoMoneda != targetNode)
        {
            targetNode = nodoMoneda;
            
            // ─── LLAMADA MODIFICADA ───
            // Le pasamos nuestro "historialNodos" al PathFinding para que los esquive
            currentPath = PathFinding.FindPath(currentNode, targetNode, historialNodos);

            // Si el camino da 0 porque la memoria bloqueó todas las opciones hacia la moneda,
            // recalculamos de emergencia SIN memoria para que el profe no se quede congelado.
            if ((currentPath == null || currentPath.Count == 0) && currentNode != targetNode)
            {
                currentPath = PathFinding.FindPath(currentNode, targetNode, null);
            }

            if (currentPath != null && currentPath.Count > 0 && !estaMoviendose)
            {
                StartCoroutine(SeguirCamino());
            }
        }
    }

    // Registra los nodos visitados en la cola tal como lo hace la moneda
    void ActualizarHistorialMemoria(Node nodoVisitado)
    {
        if (!historialNodos.Contains(nodoVisitado))
        {
            historialNodos.Enqueue(nodoVisitado);
            
            // Si superamos el tamaño de la memoria, olvidamos el más viejo
            if (historialNodos.Count > capacidadMemoria)
            {
                historialNodos.Dequeue();
            }
        }
    }

    IEnumerator SeguirCamino()
    {
        estaMoviendose = true;

        while (currentPath != null && currentPath.Count > 0)
        {
            Node n = currentPath[0];

            transform.position = new Vector3(transform.position.x, n.worldPosition.y, transform.position.z);

            while (Vector3.Distance(transform.position, n.worldPosition) > 0.05f)
            {
                Vector3 posprofePlana = new Vector3(transform.position.x, n.worldPosition.y, transform.position.z);
                Vector3 posNodoPlana = new Vector3(n.worldPosition.x, n.worldPosition.y, n.worldPosition.z);

                float distanciaAlObjetivo = Vector3.Distance(posprofePlana, posNodoPlana);
                Vector3 dir = (posNodoPlana - posprofePlana).normalized;

                float movimientoPaso = speed * Time.deltaTime;
                if (movimientoPaso > distanciaAlObjetivo)
                {
                    movimientoPaso = distanciaAlObjetivo;
                }

                // Sistema de Matrices 4D
                Vector4 currentPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
                Matrix4x4 T = Transformations.TranslateM(dir.x * movimientoPaso, 0f, dir.z * movimientoPaso);
                Vector4 newPos = T * currentPos;

                transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

                yield return null; 
            }

            currentNode = n;
            if (currentPath.Count > 0)
            {
                currentPath.RemoveAt(0);
            }
        }

        estaMoviendose = false;
    }
}