using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 initPos;

    public Vector3 dir;

    public float maxMagnitud;

    public float minMagnitud;

    public VirtualControllers.AxisButton AxisButton;

    public void OnPointerDown(PointerEventData eventData)
    {
        AxisButton.OnEnterState(dir);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AxisButton.OnExitState(dir);
        StopStick();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - initPos;

        if (direction.sqrMagnitude >= maxMagnitud * maxMagnitud)
            direction = direction.normalized * maxMagnitud;

        if (Mathf.Round(direction.sqrMagnitude) <= Mathf.Round(minMagnitud * minMagnitud))
        {
            StopStick();
            return;
        }

        transform.position = initPos + direction;

        dir.x = direction.x;
        dir.y = direction.y;

        dir /= maxMagnitud;
    }

    void StopStick()
    {
        transform.position = initPos;
        dir = Vector3.zero;
    }

    private void Update()
    {
        if(dir.sqrMagnitude>0)
            AxisButton.OnStayState(dir);
    }
}
