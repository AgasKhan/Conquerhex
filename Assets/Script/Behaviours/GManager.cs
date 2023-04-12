using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour
{
    [SerializeField] float _boundHeight;
    [SerializeField] float _boundWidth;

    public static GManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public Vector3 SetObjectBoundPosition(Vector3 pos)
    {
        float y = _boundHeight / 2;
        float x = _boundWidth / 2;

        if (pos.y > y) pos.y = -y;
        else if (pos.y < -y) pos.y = y;

        if (pos.x > x) pos.x = -x;
        else if (pos.x < -x) pos.x = x;

        return pos;
    }

    private void OnDrawGizmos()
    {
        float y = _boundHeight / 2;
        float x = _boundWidth / 2;

        Vector3 topLeft = new Vector3(-x, y, 0);
        Vector3 topRight = new Vector3(x, y, 0);
        Vector3 botRight = new Vector3(x, -y, 0);
        Vector3 botLeft = new Vector3(-x, -y, 0);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}
