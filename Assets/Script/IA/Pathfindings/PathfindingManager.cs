using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : SingletonMono<PathfindingManager>
{
    [SerializeField]
    bool showPath;

    [SerializeField]
    Color colorPath;

    [SerializeField]
    int timePathInScreen;

    public event System.Action<Vector3> newObjective;

    public void NotifyNewObjective(Vector3 pos)
    {
        newObjective(pos);
    }

    public Stack<Transform> CalculatePath(Vector3 init, Vector3 end)
    {
        var aux = ThetaStar(NodeManager.instance.GetNeighborFromPosition(end), NodeManager.instance.GetNeighborFromPosition(init));

        Stack<Transform> retorno = new Stack<Transform>();
        
        while (aux.Count > 0)
        {
            var node = aux[0];

            if (showPath && retorno.TryPeek(out var tr))
                Debug.DrawLine(tr.position, node.transform.position, colorPath, timePathInScreen);

            aux.RemoveAt(0);
            retorno.Push(node.transform);
        }
        //si deseo invertirlo
        /*
        while (aux.Count > 0)
        {
            var node = aux[aux.Count -1];

            if (showPath && retorno.TryPeek(out var tr))
                Debug.DrawLine(tr.position, node.transform.position, colorPath, timePathInScreen);

            aux.RemoveAt(aux.Count - 1);
            retorno.Push(node.transform);
        }

       
        while (aux.TryPop(out var node))
        {
            retorno.Push(node.transform);
        }
        */

        return retorno;
    }


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
                if (next.cost <= 0)
                    continue;

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


    #endregion

    public List<Node> ThetaStar(Node startingNode, Node goalNode)
    {
        if (startingNode == null || goalNode == null) return new List<Node>();

        List<Node> path = new List<Node>(AStar(startingNode, goalNode));

        int current = 0;

        while (current + 2 < path.Count)
        {
            //Si vemos ese nodo current + 2
            if (InLineOfSight(path[current].transform.position, path[current + 2].transform.position))
            {
                //Removemos el current + 1
                path.RemoveAt(current + 1);

            }
            else //Sino
            {
                current++;
            }
        }

        return path;
    }

    bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        
        var raycastHit2D=Physics2D.Raycast(start, dir, dir.magnitude, NodeManager.instance.BlockedNodeLayer);

        //Origen, direccion, distancia maxima y layer mask
        
        return raycastHit2D.collider != null ? false : true;

        //return false;
    }
}


[System.Serializable]
public class PathFinding
{
    Stack<Transform> nodes = new Stack<Transform>();

    /// <summary>
    /// posicion a la que deseo ir
    /// </summary>
    Transform objectiveVectorPos;

    /// <summary>
    /// El objetivo al que deseo ir
    /// </summary>
    Transform objective;

    Transform transform;

    [SerializeField]
    float minimalDistance=1;
    public Transform currentObjective
    {
        get
        {
            if (nodes.Count > 0)
            {
                var node = nodes.Peek();

                if ((node.position - transform.position).sqrMagnitude < minimalDistance * minimalDistance)
                {
                    nodes.Pop();
                    if (!nodes.TryPeek(out node))
                    {
                        return objective;
                    }
                }
                return node;
            }

            return objective;
        }
    }

    public void GoTo(Vector3 obj)
    {
        objectiveVectorPos.position = obj;

        objective = objectiveVectorPos;
    }

    public void GoTo(Transform obj)
    {
        objective = obj;
    }


    public void ViewOfTarget()
    {
        if (currentObjective == null)
            return;

        var aux = Physics2D.RaycastAll(transform.position, (currentObjective.position - transform.position), (currentObjective.position - transform.position).magnitude, NodeManager.instance.BlockedNodeLayer);

        if (aux != null && aux.Length > 1)
        {
            nodes = PathfindingManager.instance.CalculatePath(transform.position, currentObjective.position);
        }
    }

    public PathFinding(Transform transform)
    {
        this.transform = transform;

        objectiveVectorPos = new GameObject("objective for " + transform.name).transform;
    }
}

public interface IPathFinding
{
    Transform currentObjective { get; set; }

    Transform transform { get; }
}