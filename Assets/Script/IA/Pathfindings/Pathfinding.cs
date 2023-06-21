using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : SingletonMono<Pathfinding>
{

    public event System.Action<Vector3> newObjective;

    public void NotifyNewObjective(Vector3 pos)
    {
        newObjective(pos);
    }

    public Stack<Transform> CalculatePath(Vector3 init, Vector3 end)
    {
        var aux = AStar(NodeManager.instance.GetNeighborFromPosition(init), NodeManager.instance.GetNeighborFromPosition(end));

        Stack<Transform> retorno = new Stack<Transform>();

        foreach (var item in aux)
        {
            retorno.Push(item.transform);
        }

        return retorno;
    }

    #region Dijsktra
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
            foreach (var next in current.getNeighbors)
            {
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

            yield return timeToWait;

            if (current == goalNode)
            {
                //Si no es el mismo del inicio
                while (current != null)
                {
                    yield return timeToWait;

                    current = cameFrom[current];
                }

                break;
            }

            foreach (var next in current.getNeighbors)
            {
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
    #endregion

    #region A*
    float Heuristic(Node currentNode, Node goalNode)
    {
        return Mathf.Abs(currentNode.transform.position.x - goalNode.transform.position.x) +
                    Mathf.Abs(currentNode.transform.position.z - goalNode.transform.position.z);
    }

    public Stack<Node> AStar(Node startingNode, Node goalNode)
    {
        Stack<Node> path = new Stack<Node>();

        if (startingNode == null || goalNode == null) return path;

        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            if (current == goalNode)
            {
                while (current != null)
                {
                    path.Push(current);
                    current = cameFrom[current];
                }

                return path;
            }

            foreach (var next in current.getNeighbors)
            {
                int newCost = costSoFar[current] + next.cost;

                float priority = newCost + Heuristic(next, goalNode);

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, priority);
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next])
                {
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }
        }
        return path;
    }

    public IEnumerator PaintAStar(Node startingNode, Node goalNode)
    {
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        WaitForSeconds time = new WaitForSeconds(0.1f);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            yield return time;

            if (current == goalNode)
            {
                while (current != null)
                {
                    yield return time;
                    current = cameFrom[current];
                }

                break;
            }

            foreach (var next in current.getNeighbors)
            {

                int newCost = costSoFar[current] + next.cost;

                float priority = newCost + Heuristic(next, goalNode);

                if (!costSoFar.ContainsKey(next))
                {
                    frontier.Enqueue(next, priority);
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next])
                {
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }
        }
    }

    #endregion
}
