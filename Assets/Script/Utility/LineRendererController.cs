using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    [SerializeField]
    LineRenderer myLineRenderer;

    [SerializeField]
    List<Transform> myPoints = new List<Transform>();

    private void Awake()
    {
        myLineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        for (int i = 0; i < myPoints.Count; i++)
        {
            myLineRenderer.SetPosition(i, myPoints[i].position);
        }
    }
    void SetLine(List<Transform> _points)
    {
        myLineRenderer.positionCount = _points.Count;
        myPoints = _points;
    }

}
