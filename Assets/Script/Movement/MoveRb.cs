using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRb : MoveTr
{
    Rigidbody rb;

    public override Vector3 VectorVelocity { get => rb.velocity; set => rb.velocity = value; }

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
        rb.velocity = VelocityCalculate;

        VelocityCalculate -= _desaceleration.current * Time.fixedDeltaTime * VelocityCalculate.normalized;

        if (VelocityCalculate.sqrMagnitude <= 0)
            OnIdle();
        else
            OnMove(VectorVelocity);
    }
}
