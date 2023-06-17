using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    //Nodo será un objeto vacío

   // List<GameObject> _nodes = new List<GameObject>();

    public int[] carlitosGroup = new int[0];
    public GameObject[] carlitos = new GameObject[0];

    [SerializeField] float _viewRadius;
    [SerializeField] LayerMask obstacleLayer;

    //Node[,] _grid;

    //[SerializeField] int _width;
    //[SerializeField] int _height;

    //[SerializeField] GameObject nodePrefab;

    //[Range(1f, 5f), SerializeField]
    //float _nodeOffset;

    void Start()
    {
        //CreateGrid();
    }

    //void CreateGrid()
    //{
    //    _grid = new Node[_width, _height];

    //    for (int i = 0; i < _width; i++)
    //    {
    //        for (int j = 0; j < _height; j++)
    //        {
    //            GameObject obj = Instantiate(nodePrefab);

    //            obj.transform.position = new Vector3(i * _nodeOffset, 0, j * _nodeOffset);

    //            Node node = obj.GetComponent<Node>();

    //            node.Initialize(this, new Vector2Int(i, j));

    //            _grid[i, j] = node;
    //        }
    //    }
    //}

    public List<GameObject> GetNeighborsFromPosition()
    {
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

        

        //if (column + 1 < _height) neighbors.Add(_grid[row, column + 1]);
        //if (row + 1 < _width) neighbors.Add(_grid[row + 1, column]);
        //if (column - 1 >= 0) neighbors.Add(_grid[row, column - 1]);
        //if (row - 1 >= 0) neighbors.Add(_grid[row - 1, column]);

        return neighborsCarlitos;
    }
}

