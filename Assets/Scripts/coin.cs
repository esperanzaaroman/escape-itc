using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class coin : MonoBehaviour
{   
    public float speed = 20f;
    public GridManager gridManager;
    private Node currentNode;
    private Node targetNode;

    private Queue<Node> historialNodos = new Queue<Node>();
    public int capacidadMemoria = 6; // Cuántos nodos hacia atrás recordará
    public Node getCurrentNode(){
        return currentNode;
    }  
    void ChooseRandomNode()
    {
        List<Node> vecinos = gridManager.GetNeighbours(currentNode);
        vecinos.RemoveAll(n => !n.walkable);

        if (vecinos.Count == 0) return;

        // Filtramos los vecinos: eliminamos a cualquiera que esté en nuestra memoria
        List<Node> vecinosPermitidos = new List<Node>();
        foreach (Node v in vecinos)
        {
            if (!historialNodos.Contains(v))
            {
                vecinosPermitidos.Add(v);
            }
        }

        // Si hay opciones que no hemos visitado recientemente, elegimos una de ellas
        if (vecinosPermitidos.Count > 0)
        {
            targetNode = vecinosPermitidos[Random.Range(0, vecinosPermitidos.Count)];
        }
        else
        {
            // Si todos los vecinos ya fueron visitados (ej. un callejón o circuito cerrado),
            // no nos trabamos, simplemente elegimos el que tenga menos restricciones
            targetNode = vecinos[Random.Range(0, vecinos.Count)];
        }
    }
    void Start()
    {
        StartCoroutine(IniciarConRetraso());
    }
    IEnumerator IniciarConRetraso()
    {
        yield return null;
        if (gridManager == null)
    {
        Debug.LogError("gridManager es NULL — asígnalo en el Inspector");
        yield break;
    }

    // ¿El grid se creó?
    if (gridManager.grid == null)
    {
        Debug.LogError("gridManager.grid es NULL — Awake no corrió todavía");
        yield break;
    }

    Debug.Log("Grid size: " + gridManager.grid.GetLength(0) + " x " + gridManager.grid.GetLength(1));
        currentNode =
            gridManager.NodeFromWorldPoint(
                transform.position
            );
        if (currentNode == null)
        {
            Debug.LogError("currentNode es NULL — posición fuera del grid: " + transform.position);
            yield break;
        }
         Debug.Log("currentNode: " + currentNode.worldPosition + " | walkable: " + currentNode.walkable);

        List<Node> vecinos = gridManager.GetNeighbours(currentNode);
        Debug.Log("Vecinos encontrados: " + vecinos.Count);
        foreach (Node n in vecinos)
        {
            Debug.Log("  Vecino en " + n.worldPosition + " | walkable: " + n.walkable);
        }
        ElegirNuevoDestino();
        
        if (targetNode == null)
        {
            Debug.LogWarning("Ningún vecino es walkable");
            yield break;
        }

        // Si pasa de aquí, es seguro usarlo
        Debug.Log("Caminando hacia: " + targetNode.worldPosition);
    }
    void ElegirNuevoDestino()
    {
        if (!historialNodos.Contains(currentNode))
        {
            historialNodos.Enqueue(currentNode);
            // Si superamos el tamaño de la memoria, olvidamos el más viejo
            if (historialNodos.Count > capacidadMemoria)
            {
                historialNodos.Dequeue();
            }
        }

        ChooseRandomNode();
    }
    void Update()
    {
        
        if(targetNode == null)
            return;
        transform.position = new Vector3(
            transform.position.x,
            targetNode.worldPosition.y,
            transform.position.z
        );
        Vector3 posMonedaPlana = new Vector3(transform.position.x, targetNode.worldPosition.y, transform.position.z);
        float distanciaAlObjetivo = Vector3.Distance(posMonedaPlana, targetNode.worldPosition);
        if (distanciaAlObjetivo < 0.1f)
        {
            currentNode = targetNode;
            ElegirNuevoDestino();
            return; 
        }
        Vector3 dir = (targetNode.worldPosition - posMonedaPlana).normalized;

        float movimientoPaso = speed * Time.deltaTime;
        if (movimientoPaso > distanciaAlObjetivo)
        {
            movimientoPaso = distanciaAlObjetivo; // No te pases del nodo
        }
        Vector4 currentPos = new Vector4(
            transform.position.x,
            transform.position.y,
            transform.position.z,
            1f
        );

        
        
        Matrix4x4 T =
            Transformations.TranslateM(
                dir.x * movimientoPaso,
                0f,
                dir.z * movimientoPaso
            );

        Vector4 newPos = T * currentPos;

        transform.position =
            new Vector3(
                newPos.x,
                newPos.y,
                newPos.z
            ); 

    
    }
}
