using UnityEngine;
using System.Collections.Generic;

public class PathFinding
{
    public GridManager gridManager;

    int GetDistance(Node a, Node b)
    {
        return Mathf.Abs(a.x-b.x)+ Mathf.Abs(a.y-b.y); //Distancia Manhattan uwu
    }

    public List<Node> RetracePath(Node inicio,Node fin)
    {
        List<Node> path =
            new List<Node>(); //crea path vacio

        Node current = fin; //saca el destinichis

        while (current != inicio) //como la clase nodo guarda el papa como el nodo anterior hay que recorrer al reves
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse(); //saca el path ya bien de inicio-fin

        foreach(Node n in path)
        {
            Debug.Log($"{n.x},{n.y}"); //xd
        }
        return path;
    }
    public List<Node> FindPath(Node startNode, Node targetNode, Queue<Node> historialProfe)
    {
        if (startNode == null || targetNode == null) 
        return new List<Node>();

        List<Node> openSet = //lista de nodos a explorar (las hojitas del grafo) los vecinos cercanos vaya
            new List<Node>();
        
        HashSet<Node> closedSet = //los q ya exploramos, para que ho se vaya a ciclar (es un hash set para que excluya los duplicados y la busqueda sea O(1))
            new HashSet<Node>();

        openSet.Add(startNode); //empieza con el start

        while(openSet.Count > 0) //mientras no este vacia (haya nodos por explorar)
        {
            Node currentNode=openSet[0]; //saca el primer nodo del open set, creo que esta mas facil no usar un ordered porque hay que ponerle la key para q se ordene y comnfunde 

            foreach(Node nodo in openSet) //checa para agarra el node con el costo mas bajito cute
            {
                if(nodo.FCost<currentNode.FCost)
                    currentNode=nodo;
            }

            openSet.Remove(currentNode); //borra el nodo elegido de open

            closedSet.Add(currentNode); //lo pone el closed

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode); 
            }
            foreach(Node vecino in 
                gridManager.GetNeighbours(currentNode) //saca sus vecinichos (visiten la funcion en grid manager)
            )
            {
                if(!vecino.walkable||closedSet.Contains(vecino)|| (historialProfe != null && historialProfe.Contains(vecino))) // checa si no es pared (walkable) o si ya lo visitamos 
                    continue; //si pasa el if se salta al sig vecino (no reviso si ya llegue al mejor de los closed porque manhattan es padre y eso no ocurre (en teoria))

                int nuevoCosto = currentNode.gCost+GetDistance(currentNode,vecino); //crea el costo con dist manhattan

                if(nuevoCosto<vecino.gCost
                    || !openSet.Contains(vecino) // si el costo de llegar al vecino es mas chiquito que el que ya tenia (osea soy mas pro esta vez) o si no esta en open set y no me importa el costo xd
                )
                {
                    vecino.gCost=nuevoCosto; //le doy el costo de llegar hasta el vecino el mas chiquito que el que tenia uwu

                    vecino.hCost=GetDistance(vecino,targetNode); //saca euristica (mahattan)

                    vecino.Parent=currentNode; //mi papa es el vecino por donde llegue jeje

                    if(!openSet.Contains(vecino)) //si no estan en open lo pongo (esto es por si econtre una ruta mejor y ya lo habia explorado xd)
                        openSet.Add(vecino);
                }
            }


        }
        return new List<Node>();
    }
}
