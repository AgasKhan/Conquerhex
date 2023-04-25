using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundaries : MonoBehaviour
{
    [SerializeField] float _boundHeight;
    [SerializeField] float _boundWidth;

    MoveAbstract move;
    public static Boundaries instance { get; private set; }

    void Awake()
    {
        instance = this;
    }

    public Vector3 SetObjectBoundPosition(Vector3 pos)
    {
        float y = _boundHeight / 2;
        float x = _boundWidth / 2;

        if (pos.y > y -1) move.Velocity(move.velocity * -1); //pos.y = -y;
        else if (pos.y < -y +1) move.Velocity(move.velocity * 1); //pos.y = y;

        if (pos.x > x -1) move.Velocity(move.velocity * -1); //pos.x = -x;
        else if (pos.x < -x +1) move.Velocity(move.velocity * 1); //pos.x = x;

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
