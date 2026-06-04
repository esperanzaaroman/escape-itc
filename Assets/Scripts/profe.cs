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

    // Memoria usando únicamente la cola tradicional
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

        // CORRECCIÓN CRÍTICA: Solo permitimos recalcular si se cumplió el tiempo 
        // Y ADEMÁS el Profe no está en medio de un movimiento intermedio entre nodos.
        if (cronometro >= tiempoEntreRecalculos && corrutinaMovimiento == null)
        {
            Node nodoMoneda = objetivo.getCurrentNode();

            if (nodoMoneda != null && nodoMoneda != targetNode)
            {
                targetNode = nodoMoneda;
                
                // Obtenemos nuestra posición real exacta en la cuadrícula antes de recalcular
                Node nodoActualReal = gridManager.NodeFromWorldPoint(transform.position);
                if (nodoActualReal != null)
                {
                    currentNode = nodoActualReal;
                }

                // Solicitamos la ruta pasando la cola directamente
                currentPath = PathFinding.FindPath(currentNode, targetNode, historialNodos);

                // Amnesia de emergencia si la memoria bloqueó todos los accesos posibles
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

            // Alineamos la altura para evitar desvíos en el eje Y
            transform.position = new Vector3(transform.position.x, n.worldPosition.y, transform.position.z);

            // Bucle que asegura que el personaje REALMENTE llegue al centro del cuadro físico
            while (Vector3.Distance(new Vector3(transform.position.x, n.worldPosition.y, transform.position.z), n.worldPosition) > 0.08f)
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

                // Tu sistema de transformación 4D
                Vector4 currentPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
                Matrix4x4 T = Transformations.TranslateM(dir.x * movimientoPaso, 0f, dir.z * movimientoPaso);
                Vector4 newPos = T * currentPos;

                transform.position = new Vector3(newPos.x, newPos.y, newPos.z);

                yield return null; 
            }

            // CORRECCIÓN HISTÓRICA: Ahora que garantizamos que llegó, guardamos el nodo que dejamos atrás
            ActualizarHistorialMemoria(currentNode);

            // Actualizamos formalmente nuestra posición actual con el nodo que acabamos de pisar
            currentNode = n;

            if (currentPath.Count > 0)
            {
                currentPath.RemoveAt(0);
            }
        }

        // Liberamos el estado de movimiento para permitir que el Update vuelva a buscar a la moneda
        corrutinaMovimiento = null;
    }
}