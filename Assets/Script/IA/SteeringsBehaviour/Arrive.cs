using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    protected override Vector2 InternalCalculate(MoveAbstract target)
    {
        Vector2 desired = Direction(target);
        var speed = me.maxSpeed;

        if (desired.sqrMagnitude <= (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
            speed = me.maxSpeed * ((desired.magnitude - 1) / me._desaceleration.current);

        desired *= speed;

        Vector2 st = desired - me.vectorVelocity;
        st = Vector2.ClampMagnitude(st, me.maxSpeed);

        return st;

    }
}
