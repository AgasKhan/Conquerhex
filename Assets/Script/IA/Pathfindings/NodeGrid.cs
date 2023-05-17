using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    //Nodo será un objeto vacío

    Node[,] _grid;

    [SerializeField] int _width;
    [SerializeField] int _height;

    [SerializeField] GameObject nodePrefab;

    [Range(1f, 5f), SerializeField]
    float _nodeOffset;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new Node[_width, _height];

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                GameObject obj = Instantiate(nodePrefab);

                obj.transform.position = new Vector3(i * _nodeOffset, 0, j * _nodeOffset);

                Node node = obj.GetComponent<Node>();

                node.Initialize(this, new Vector2Int(i, j));

                _grid[i, j] = node;
            }
        }
    }

    public List<Node> GetNeighborsFromPosition(int row, int column)
    {
        List<Node> neighbors = new List<Node>();

        if (column + 1 < _height) neighbors.Add(_grid[row, column + 1]);
        if (row + 1 < _width) neighbors.Add(_grid[row + 1, column]);
        if (column - 1 >= 0) neighbors.Add(_grid[row, column - 1]);
        if (row - 1 >= 0) neighbors.Add(_grid[row - 1, column]);

        return neighbors;
    }
}

