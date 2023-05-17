using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : SingletonMono<PathManager>
{
    //Me interesa sacar los colores y controles. Potencial clase a matar con el tiempo

    //public static PathManager Instance { get; private set; }

    //[SerializeField] Agent _myAgent;

    Node _startingNode;
    Node _goalNode;

    Pathfinding _pathfinding;

    //void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else Destroy(gameObject);

    //    _pathfinding = new Pathfinding();
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_startingNode != null)
            {
                StartCoroutine(_pathfinding.PaintDijkstra(_startingNode, _goalNode));
                //_pathfinding.BFS(_startingNode, _goalNode);
            }
        }
    }

    public void SetStartingNode(Node n)
    {
        if (_startingNode != null) ChangeObjColor(_startingNode.gameObject, Color.white);

        _startingNode = n;

        ChangeObjColor(_startingNode.gameObject, Color.blue);

        //if (_myAgent) _myAgent.SetStartNode(n);

    }

    public void SetGoalNode(Node node)
    {
        if (_goalNode != null) ChangeObjColor(_goalNode.gameObject, Color.white);

        _goalNode = node;

        ChangeObjColor(_goalNode.gameObject, Color.green);

        //if (_myAgent) _myAgent.SetGoalNode(node);
    }

    public void ChangeObjColor(GameObject obj, Color color)
    {
        obj.GetComponent<Renderer>().material.color = color;
    }
}
