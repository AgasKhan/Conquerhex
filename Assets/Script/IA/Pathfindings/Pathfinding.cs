using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public Stack<Node> Dijkstra(Node startingNode, Node goalNode)
    {
        //Genero una lista donde voy a guardar cada Nodo que genere el camino
        Stack<Node> path = new Stack<Node>();

        if (startingNode == null || goalNode == null) return path;

        //Inicializo una PriorityQueue y agregamos nuestro nodo de comienzo
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        //Inicializo un dict donde voy a guardar el nodo del cual viene cada nodo y agrego mi nodo de inicio que proviene de nadie
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        //Inicializo mi dict donde guardo el nodo y el costo para llegar a el (el nodo del comienzo siempre es costo 0)
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        //Mientras frontier tenga nodos dentro
        while (frontier.Count > 0)
        {
            //Obtenemos el primero de la queue
            Node current = frontier.Dequeue();

            //Si se llego al Nodo destino
            if (current == goalNode)
            {
                //Si no es el mismo del inicio
                while (current != null)
                {
                    //Lo agrego al camino
                    path.Push(current);

                    //Tomo el nodo del cual viene
                    current = cameFrom[current];
                }

                return path;
            }

            //Analizamos sus vecinos
            foreach (var next in current.GetNeighbors())
            {
                if (next.IsBlocked) continue;

                //Obtenemos el costo total entre el costo actual que nos devuelve nuestro costSoFar
                //y el costo del next
                int newCost = costSoFar[current] + next.cost;

                if (!costSoFar.ContainsKey(next))
                {
                    //Le agregamos a frontier el nodo actual y el peso
                    frontier.Enqueue(next, newCost);

                    //Agregamos al diccionario el nodo vecino y de cual esta viniendo
                    cameFrom.Add(next, current);

                    //Agregamos al diccionario el nodo y su peso actual
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next]) //Si el costoNuevo en base a este nodo chequeado es menor al que ya tenia
                {
                    //Lo piso en la Queue
                    frontier.Enqueue(next, newCost);
                    //Lo piso en el dict de donde viene
                    cameFrom[next] = current;
                    //Lo piso en el dict de nodo + peso actual
                    costSoFar[next] = newCost;
                }
            }
        }
        return path;
    }


    public IEnumerator PaintDijkstra(Node startingNode, Node goalNode)
    {
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        WaitForSeconds timeToWait = new WaitForSeconds(0.05f);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            PathManager.instance.ChangeObjColor(current.gameObject, Color.blue);

            yield return timeToWait;

            if (current == goalNode)
            {
                //Si no es el mismo del inicio
                while (current != null)
                {
                    PathManager.instance.ChangeObjColor(current.gameObject, Color.yellow);

                    yield return timeToWait;

                    current = cameFrom[current];
                }

                break;
            }

            foreach (var next in current.GetNeighbors())
            {
                if (next.IsBlocked) continue;

                int newCost = costSoFar[current] + next.cost;

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, newCost);
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next])
                {
                    frontier.Enqueue(next, newCost);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }
        }
    }
}
