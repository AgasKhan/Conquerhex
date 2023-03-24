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

    public float timer;

    public event System.Action<Vector2, float> down;
    public event System.Action<Vector2, float> pressed;
    public event System.Action<Vector2, float> up;

    public void OnPointerDown(PointerEventData eventData)
    {
        down(dir, timer);
        timer = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        up(dir, timer);
        StopStick();
        timer = -1;
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

        Debug.Log(direction.magnitude);

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
        if(timer>-1)
            timer += Time.deltaTime;
        pressed(dir, timer);
    }
}
