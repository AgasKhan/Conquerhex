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

    protected void MyUpdate()
    {
        transform.position += (direction * _velocity.current * Time.deltaTime).Vec2to3(0);

        _velocity.Substract(_desaceleration.current * Time.deltaTime);

        if (_velocity.current <= 0)
            OnIdle();
        else
            OnMove(direction * _velocity.current);
    }


    void MyUpdateRender()
    {
        rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        //rend.flipX = ! (Vector2.Dot(vectorVelocity, Vector2.right) > 0.5) && ;
            
    }

}
