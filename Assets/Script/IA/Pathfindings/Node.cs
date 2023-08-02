using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    [SerializeField]
    List<Node> _neighbors = new List<Node>();

    [SerializeField]
    Color colorPaths = new Color { a=1,b=1,g=1,r=1 };

    public int cost = 1;

    public List<Node> getNeighbors => _neighbors;

    private void Start()
    {
        NodeManager.instance.saveNode = this;
    }

    /// <summary>
    /// Comentar cuando no este en desarrollo el escenario
    /// </summary>
    private void OnDrawGizmos()
    {
        if(cost <= 0)
            return;

        Gizmos.color = colorPaths;
        
        for (int i = 0; i < _neighbors.Count; i++)
        {
            if (_neighbors[i].cost <= 0)
                continue;

            if(_neighbors[i].getNeighbors.Contains(this))
                Gizmos.color = colorPaths;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, _neighbors[i].transform.position);
        }
    }
}
