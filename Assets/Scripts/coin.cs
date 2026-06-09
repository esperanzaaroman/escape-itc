using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class coin : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 20f;
    public GridManager gridManager;

    private Node currentNode;
    private Node targetNode;
    private bool _initialized = false;

    private Queue<Node> historialNodos = new Queue<Node>();
    public int capacidadMemoria = 6;

    // ── Eventos que CoinManager escucha ───────────────────────────────────────
    public static event System.Action<coin> OnCoinCollectedByPlayer;
    public static event System.Action<coin> OnCoinCollectedByProfe;
    private bool _collected = false;

    // ── API que usa profe.cs ──────────────────────────────────────────────────
    public Node getCurrentNode() => currentNode;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        StartCoroutine(IniciarConRetraso());
    }

    IEnumerator IniciarConRetraso()
    {
        yield return null;

        if (gridManager == null)
        {
            Debug.LogError("[coin] gridManager es NULL — asígnalo en el Inspector");
            yield break;
        }
        if (gridManager.grid == null)
        {
            Debug.LogError("[coin] gridManager.grid es NULL — Awake no corrió todavía");
            yield break;
        }

        currentNode = gridManager.NodeFromWorldPoint(transform.position);
        if (currentNode == null)
        {
            Debug.LogError("[coin] currentNode es NULL — posición fuera del grid: " + transform.position);
            yield break;
        }

        _initialized = true;
        ElegirNuevoDestino();
    }

    // ── Lógica de navegación aleatoria ────────────────────────────────────────
    void ChooseRandomNode()
    {
        List<Node> vecinos = gridManager.GetNeighbours(currentNode);
        vecinos.RemoveAll(n => !n.walkable);
        if (vecinos.Count == 0) return;

        List<Node> permitidos = new List<Node>();
        foreach (Node v in vecinos)
            if (!historialNodos.Contains(v))
                permitidos.Add(v);

        targetNode = (permitidos.Count > 0)
            ? permitidos[Random.Range(0, permitidos.Count)]
            : vecinos[Random.Range(0, vecinos.Count)];
    }

    void ElegirNuevoDestino()
    {
        if (!historialNodos.Contains(currentNode))
        {
            historialNodos.Enqueue(currentNode);
            if (historialNodos.Count > capacidadMemoria)
                historialNodos.Dequeue();
        }
        ChooseRandomNode();
    }

    // ── Movimiento (BUG CORREGIDO: ya no fuerza Y al nodo) ───────────────────
    void Update()
    {
        if (!_initialized || targetNode == null) return;

        // Destino en XZ, conservando la Y propia del objeto
        Vector3 destino = new Vector3(
            targetNode.worldPosition.x,
            transform.position.y,
            targetNode.worldPosition.z
        );

        float distancia = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(destino.x,            0f, destino.z)
        );

        if (distancia < 0.05f)
        {
            // Snap exacto al centro del nodo en XZ
            transform.position = new Vector3(
                targetNode.worldPosition.x,
                transform.position.y,
                targetNode.worldPosition.z
            );
            currentNode = targetNode;
            ElegirNuevoDestino();
            return;
        }

        Vector3 dir = (destino - transform.position);
        dir.y = 0f;
        dir.Normalize();

        float paso = Mathf.Min(speed * Time.deltaTime, distancia); // Anti-overshoot

        Vector4 pos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
        Matrix4x4 T = Transformations.TranslateM(dir.x * paso, 0f, dir.z * paso);
        Vector4 newPos = T * pos;

        transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
    }

    // ── Recolección por trigger (jugador con tag "Player") ────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (other.CompareTag("Player"))
            Collect();
    }

    // ── Recolección por nodo (llamada desde Player.cs cuando comparte nodo) ───
    public void CollectFromNode()
    {
        if (!_collected) Collect();
    }

    private void Collect()
    {
        _collected = true;
        OnCoinCollectedByPlayer?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 0.05f);
    }

    public void CollectByProfe()
    {
        if (_collected) return;
        _collected = true;
        OnCoinCollectedByProfe?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 0.05f);
    }

    public bool IsCollected => _collected;
}