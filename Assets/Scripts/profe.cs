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

    [Header("Profundidad de búsqueda A*")]
    [Tooltip("Máximo de nodos que el A* expande por búsqueda.\n" +
             "5  = muy limitado, comportamiento casi aleatorio.\n" +
             "20 = equilibrio recomendado.\n" +
             "-1 = sin límite (búsqueda completa).")]
    public int profundidadBusqueda = 20;

    [Header("Detección de captura")]
    public float radioCaptura = 1.5f;
    private GameManager _gameManager;
    private CharacterMovement _player;
    private bool _juegoTerminado = false;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        PathFinding = new PathFinding();
        PathFinding.gridManager = this.gridManager;

        _gameManager = FindObjectOfType<GameManager>();
        _player      = FindObjectOfType<CharacterMovement>();

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
        if (_juegoTerminado) return;
        if (gridManager == null || gridManager.grid == null || objetivo == null) return;

        // ── Detección de captura por distancia 
        if (_player != null)
        {
            float dist = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(_player.transform.position.x, 0f, _player.transform.position.z)
            );

            if (dist <= radioCaptura)
            {
                _juegoTerminado = true;
                _gameManager?.MostrarDerrota();
                return;
            }
        }

        // ── Pathfinding hacia la moneda
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreRecalculos && corrutinaMovimiento == null)
        {
            Node nodoMoneda = objetivo.getCurrentNode();

            if (nodoMoneda != null && nodoMoneda != targetNode)
            {
                targetNode = nodoMoneda;

                Node nodoActualReal = gridManager.NodeFromWorldPoint(transform.position);
                if (nodoActualReal != null)
                    currentNode = nodoActualReal;

                // Búsqueda con profundidad limitada
                currentPath = PathFinding.FindPath(currentNode, targetNode,
                                                   historialNodos, profundidadBusqueda);

                // Si no encontró nada con historial, reintenta sin restricción de memoria
                if ((currentPath == null || currentPath.Count == 0) && currentNode != targetNode)
                    currentPath = PathFinding.FindPath(currentNode, targetNode,
                                                       null, profundidadBusqueda);

                if (currentPath != null && currentPath.Count > 0)
                    corrutinaMovimiento = StartCoroutine(SeguirCamino());
            }

            cronometro = 0f;
        }
    }

    void ActualizarHistorialMemoria(Node nodoVisitado)
    {
        if (nodoVisitado == null) return;
        if (!historialNodos.Contains(nodoVisitado))
            historialNodos.Enqueue(nodoVisitado);
        if (historialNodos.Count > capacidadMemoria)
            historialNodos.Dequeue();
    }

    IEnumerator SeguirCamino()
    {
        while (currentPath != null && currentPath.Count > 0)
        {
            Node n = currentPath[0];
            Vector3 destino = new Vector3(n.worldPosition.x, transform.position.y, n.worldPosition.z);

            while (true)
            {
                float distancia = Vector3.Distance(
                    new Vector3(transform.position.x, 0f, transform.position.z),
                    new Vector3(destino.x,            0f, destino.z)
                );
                if (distancia <= 0.05f) break;

                Vector3 dir = (destino - transform.position);
                dir.y = 0f;
                dir.Normalize();

                float paso = Mathf.Min(speed * Time.deltaTime, distancia);

                Vector4 pos    = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
                Matrix4x4 T    = Transformations.TranslateM(dir.x * paso, 0f, dir.z * paso);
                Vector4 newPos = T * pos;
                transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

                yield return null;
            }

            transform.position = new Vector3(n.worldPosition.x, transform.position.y, n.worldPosition.z);
            ActualizarHistorialMemoria(currentNode);
            currentNode = n;
            currentPath.RemoveAt(0);
        }

        corrutinaMovimiento = null;
    }
}