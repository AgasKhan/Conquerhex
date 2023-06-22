using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : SingletonMono<NodeManager>
{
    [SerializeField] float _viewRadius;
    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] List<Node> nodesList = new List<Node>();

    public Node saveNode
    {
        set
        {
            nodesList.Add(value);
        }
    }

    public Node GetNeighborFromPosition(Vector3 pos)
    {

        List<Node> nearest = new List<Node>(nodesList);

        float distance = float.MaxValue;

        Node retorno = null;

        for (int i = nearest.Count - 1; i >= 0; i--)
        {
            if (Physics2D.Raycast(pos, (nearest[i].transform.position - pos), (nearest[i].transform.position - pos).magnitude, obstacleLayer))
            {
                nearest.RemoveAt(i);
            }
        }        

        for (int i = 0; i < nearest.Count; i++)
        {
            if(distance > (nearest[i].transform.position - pos).sqrMagnitude)
            {
                retorno = nearest[i];
                distance = (nearest[i].transform.position - pos).sqrMagnitude;
            }
        }

        /*
        int id = 0;
        List<GameObject> neighborsCarlitos = new List<GameObject>();

        if (Physics.Raycast(transform.position, carlitos[id].transform.position, _viewRadius, obstacleLayer))
        {
            for (int i = 0; i < carlitos.Length; i++)
            {
                if(!neighborsCarlitos.Contains(carlitos[id]))
                     neighborsCarlitos.Add(carlitos[id]);

                id++;
            }
        }
        */

        return retorno;
    }
}

