using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class profe : MonoBehaviour
{
    public coin objetivo;
    public float speed = 10f;
    public GridManager gridManager;
    public PathFinding PathFinding;
    
    private Node currentNode;
    private Node targetNode;
    private List<Node> currentPath = new List<Node>();

    private Coroutine corrutinaMovimiento;

    private Queue<Node> historialNodos = new Queue<Node>();
    public int capacidadMemoria = 5; 

    [Header("Configuración de Optimización")]
    public float tiempoEntreRecalculos = 0.5f; 
    private float cronometro = 0f; 

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

        cronometro += Time.deltaTime;

        // Cambiamos el rumbo únicamente cuando se cumpla el tiempo y la corrutina actual termine
        if (cronometro >= tiempoEntreRecalculos && corrutinaMovimiento == null)
        {
            Node nodoMoneda = objetivo.getCurrentNode();

            if (nodoMoneda != null && nodoMoneda != targetNode)
            {
                targetNode = nodoMoneda;
                
                Node nodoActualReal = gridManager.NodeFromWorldPoint(transform.position);
                if (nodoActualReal != null)
                {
                    currentNode = nodoActualReal;
                }

                currentPath = PathFinding.FindPath(currentNode, targetNode, historialNodos);

                if ((currentPath == null || currentPath.Count == 0) && currentNode != targetNode)
                {
                    currentPath = PathFinding.FindPath(currentNode, targetNode, null);
                }

                if (currentPath != null && currentPath.Count > 0)
                {
                    corrutinaMovimiento = StartCoroutine(SeguirCamino());
                }
            }

            cronometro = 0f;
        }
    }

    void ActualizarHistorialMemoria(Node nodoVisitado)
    {
        if (nodoVisitado == null) return;

        if (!historialNodos.Contains(nodoVisitado))
        {
            historialNodos.Enqueue(nodoVisitado);
        }

        if (historialNodos.Count > capacidadMemoria)
        {
            historialNodos.Dequeue();
        }
    }

    IEnumerator SeguirCamino()
    {
        while (currentPath != null && currentPath.Count > 0)
        {
            Node n = currentPath[0];

            // Destino plano (ignoramos Y para no hundir/elevar al profe)
            Vector3 destino = new Vector3(n.worldPosition.x, transform.position.y, n.worldPosition.z);

            // Nos acercamos hasta estar dentro del umbral de llegada
            while (true)
            {
                Vector3 posActual = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                float distancia = Vector3.Distance(
                    new Vector3(posActual.x, 0f, posActual.z),
                    new Vector3(destino.x,   0f, destino.z)
                );

                if (distancia <= 0.05f) break;

                Vector3 dir = (destino - posActual);
                dir.y = 0f;
                dir.Normalize();

                float paso = speed * Time.deltaTime;
                if (paso > distancia) paso = distancia; // Anti-overshoot

                Vector4 currentPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
                Matrix4x4 T = Transformations.TranslateM(dir.x * paso, 0f, dir.z * paso);
                Vector4 newPos = T * currentPos;

                transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

                yield return null;
            }

            // Alineación exacta en XZ al centro del nodo; conservamos la Y del profe
            transform.position = new Vector3(n.worldPosition.x, transform.position.y, n.worldPosition.z);

            ActualizarHistorialMemoria(currentNode);
            currentNode = n;

            if (currentPath.Count > 0)
            {
                currentPath.RemoveAt(0);
            }
        }
        corrutinaMovimiento = null;
    }
}