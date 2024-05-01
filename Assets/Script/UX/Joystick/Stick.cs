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

    public Controllers.Axis AxisButton;

    bool press;

    public void OnPointerDown(PointerEventData eventData)
    {
        AxisButton.OnEnterState(dir);
        press = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AxisButton.OnExitState(dir);
        StopStick();
        press = false;
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
        transform.localPosition = Vector3.zero;
        dir = Vector3.zero;
    }

    private void Update()
    {
        if(press)
            AxisButton.OnStayState(dir);
    }
}
