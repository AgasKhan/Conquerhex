using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTr : MoveAbstract
{
    SpriteRenderer rend;

    protected override void Config()
    {
        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;

        if (rend != null)
            MyUpdates += MyUpdateRender;
    }

    void MyAwake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();

        if (rend != null)
            MyUpdates += MyUpdateRender;
    }

    public void MyUpdate()
    {
        transform.position += (direction * _velocity.current * Time.deltaTime).Vec2to3(0);

        _velocity.Substract(_desaceleration.current * Time.deltaTime);
    }

    void MyUpdateRender()
    {
        rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }

}
