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

    [Header("Recolección de monedas")]
    public float radioRecoleccion = 1.0f;

    [Header("Línea de visión")]
    [Tooltip("Distancia máxima a la que el profe puede ver al jugador")]
    public float radioVision = 8f;
    [Tooltip("LayerMask de las paredes — el mismo wallMask del GridManager")]
    public LayerMask capasParedes;
    private bool _veAlJugador = false;

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

        // ── Recolección de moneda por proximidad
        if (objetivo != null && !objetivo.IsCollected)
        {
            float distMoneda = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(objetivo.transform.position.x, 0f, objetivo.transform.position.z)
            );
            if (distMoneda <= radioRecoleccion)
                objetivo.CollectByProfe();
        }

        // ── Línea de visión al jugador
        _veAlJugador = false;
        if (_player != null)
        {
            Vector3 origen    = new Vector3(transform.position.x, 1f, transform.position.z);
            Vector3 destino   = new Vector3(_player.transform.position.x, 1f, _player.transform.position.z);
            Vector3 direccion = destino - origen;
            float distancia   = direccion.magnitude;

            Debug.Log($"[Visión] Player: {(_player == null ? "NULL" : "OK")} | Dist: {distancia:F1} | RadioVision: {radioVision}");
            if (distancia <= radioVision)
            {
                RaycastHit hit;
                bool choco = Physics.Raycast(origen, direccion.normalized, out hit, distancia, capasParedes);
                Debug.Log($"[Visión] Chocó: {choco} | Objeto: {(choco ? hit.collider.name : "ninguno")}");
                if (!choco)
                    _veAlJugador = true;
            }
        }

        // ── Pathfinding hacia moneda o jugador
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreRecalculos)
        {
            Node nodoObjetivo = _veAlJugador
                ? gridManager.NodeFromWorldPoint(_player.transform.position)
                : objetivo.getCurrentNode();

            if (nodoObjetivo != null && (nodoObjetivo != targetNode || _veAlJugador))
            {
                // Interrumpir camino actual para recalcular
                if (corrutinaMovimiento != null)
                {
                    StopCoroutine(corrutinaMovimiento);
                    corrutinaMovimiento = null;
                }

                targetNode = nodoObjetivo;

                Node nodoActualReal = gridManager.NodeFromWorldPoint(transform.position);
                if (nodoActualReal != null)
                    currentNode = nodoActualReal;

                Queue<Node> historial = _veAlJugador ? null : historialNodos;
                currentPath = PathFinding.FindPath(currentNode, targetNode,
                                                   historial, profundidadBusqueda);

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

                // Rotar el profe hacia donde se mueve (solo eje Y)
                if (dir != Vector3.zero)
                {
                    float anguloY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 90f;
                    transform.rotation = Quaternion.Euler(
                        transform.rotation.eulerAngles.x,
                        anguloY,
                        transform.rotation.eulerAngles.z
                    );
                }

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