using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{
    protected override Vector3 InternalCalculate(MoveAbstract target)
    {
        Vector3 desired = Direction(target);
        var speed = me.maxSpeed;

        _desiredVelocity = Vector3.ClampMagnitude(desired, me.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
            _desiredVelocity = -me.vectorVelocity * (me._desaceleration.current - 1);

        _steering = _desiredVelocity - me.vectorVelocity;

        return _steering;
    }
}
