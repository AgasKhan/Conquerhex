using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRb : MoveTr
{
    Rigidbody rb;

    public override Vector3 vectorVelocity => rb.velocity;

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
        rb.velocity = (direction * _velocity.current);
        _velocity.Substract(_desaceleration.current * Time.fixedDeltaTime);

        if (_velocity.current <= 0)
            OnIdle();
        else
            OnMove(rb.velocity);
    }
}
