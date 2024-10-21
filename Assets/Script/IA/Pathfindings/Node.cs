using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface INode<TChild> where TChild: INode<TChild>
{
    public IEnumerable<TChild> GetNeighbors { get; }
}

public class Node : MonoBehaviour, INode<Node>
{
    [SerializeField]
    List<Node> _neighbors = new List<Node>();

    [SerializeField]
    Color colorPaths = new Color { a=1,b=1,g=1,r=1 };

    public int cost = 1;

    public IEnumerable<Node> GetNeighbors => _neighbors;

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

            if(_neighbors[i].GetNeighbors.Contains(this))
                Gizmos.color = colorPaths;
            else
                Gizmos.color = Color.red;

            Utilitys.DrawArrowLine(transform.position, _neighbors[i].transform.position);
        }
    }
}
