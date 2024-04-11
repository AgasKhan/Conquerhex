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
    }

    void MyAwake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void MyFixedUpdate()
    {
        rb.velocity = (direction * _velocity.current).Vec2to3(0);
        _velocity.Substract(_desaceleration.current * Time.fixedDeltaTime);

        if (_velocity.current <= 0)
            OnIdle();
        else
            OnMove(rb.velocity);
    }
}
