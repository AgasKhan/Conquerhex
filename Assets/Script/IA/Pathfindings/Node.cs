using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{

    List<Node> _neighbors = new List<Node>();

    NodeGrid _grid;

    Vector2Int _gridPosition;

    public int cost = 1;

    public bool IsBlocked { get; private set; }

    public TextMeshProUGUI costText;

    public void Initialize(NodeGrid grid, Vector2Int gridPosition)
    {
        _grid = grid;
        _gridPosition = gridPosition;
        costText.text = "1";
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            PathManager.instance.SetStartingNode(this);
        else if (Input.GetMouseButtonDown(1))
            PathManager.instance.SetGoalNode(this);
        else if (Input.GetMouseButtonDown(2))
            SetBlocked(!IsBlocked);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetCost(cost == 1 ? cost + 4 : cost + 5);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetCost(cost - 5);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetCost(1);
        }
    }

    void SetCost(int c)
    {
        cost = Mathf.Clamp(c, 1, 99);

        costText.text = cost.ToString();

        PathManager.instance.ChangeObjColor(gameObject, Color.Lerp(Color.white, Color.green, cost / 50f));
    }

    public List<Node> GetNeighbors()
    {
        if (_neighbors.Count == 0)
            _neighbors = _grid.GetNeighborsFromPosition(_gridPosition.x, _gridPosition.y);

        return _neighbors;
    }

    void SetBlocked(bool isBlock)
    {
        IsBlocked = isBlock;
        Color color = isBlock ? Color.black : Color.white;

        PathManager.instance.ChangeObjColor(gameObject, color);
    }

    public void CheckNode()
    {
        PathManager.instance.ChangeObjColor(gameObject, Color.blue);
    }

    public void PathNode()
    {
        PathManager.instance.ChangeObjColor(gameObject, Color.yellow);
    }
}
