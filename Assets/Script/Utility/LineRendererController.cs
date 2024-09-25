using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    [SerializeField]
    LineRenderer myLineRenderer;

    [SerializeField]
    List<Transform> dynamicPoints = new List<Transform>();

    private void Awake()
    {
        myLineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            myLineRenderer.SetPosition(i, dynamicPoints[i].position);
        }
    }
    void SetLine(List<Transform> _points)
    {
        myLineRenderer.positionCount = _points.Count;
        dynamicPoints = _points;
    }

}
