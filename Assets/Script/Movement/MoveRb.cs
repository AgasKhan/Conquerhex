using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRb : MoveTr
{
    Rigidbody2D rb;

    public override Vector2 vectorVelocity => rb.velocity;

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyFixedUpdates += MyFixedUpdate;
    }


    void MyAwake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void MyFixedUpdate()
    {
        rb.velocity = (direction * _velocity.current).Vec2to3(0);
    }

    public override void MyUpdate()
    {
        _velocity.Substract(_desaceleration.current * Time.deltaTime);
    }

}
