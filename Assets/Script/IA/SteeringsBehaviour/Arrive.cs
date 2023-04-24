using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    public override Vector2 Calculate(MoveAbstract target)
    {
        _desiredVelocity = Vector2.ClampMagnitude(target.transform.position, move.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (move.velocity * move.velocity / (move._desaceleration.current * move._desaceleration.current)))
            _desiredVelocity = -move.vectorVelocity * (move._desaceleration.current - 1);

        _steering = _desiredVelocity - move.vectorVelocity;

        var vecVelocity = move.vectorVelocity;

        vecVelocity += _steering * Time.deltaTime;

        return vecVelocity;
    }
}
