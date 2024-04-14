using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRb : MoveTr
{
    Rigidbody rb;

    public override Vector2 vectorVelocity => rb.velocity.Vect3To2XZ();

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void MyFixedUpdate()
    {
        rb.velocity = (direction * _velocity.current).Vect2To3XZ(0);
        _velocity.Substract(_desaceleration.current * Time.fixedDeltaTime);

        if (_velocity.current <= 0)
            OnIdle();
        else
            OnMove(rb.velocity);
    }
}
