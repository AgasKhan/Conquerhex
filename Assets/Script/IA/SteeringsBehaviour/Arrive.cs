using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    public override Vector2 Calculate(MoveAbstract target)
    {
        _desiredVelocity = Vector2.ClampMagnitude(target.transform.position, me.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
            _desiredVelocity = -me.vectorVelocity * (me._desaceleration.current - 1);

        _steering = _desiredVelocity - me.vectorVelocity;

        var vecVelocity = me.vectorVelocity;

        vecVelocity += _steering * Time.deltaTime;

        return vecVelocity;
    }
}
