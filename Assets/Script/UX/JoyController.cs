using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyController : Controlador, IDragHandler, IEndDragHandler
{
    [SerializeField] float maxMagnitud = 60f;

    Vector2 initPos;

    Vector3 dir;

    public override Vector3 MoveDir()
    {
        return dir / maxMagnitud;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 direction = Vector2.ClampMagnitude(eventData.position - initPos, maxMagnitud);

        transform.position = initPos + direction;

        dir.x = direction.x;
        dir.y = direction.y;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        transform.position = initPos;
        dir = Vector3.zero;
    }

    private void Start()
    {
        initPos = transform.position;


    }

}
