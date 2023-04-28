using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{

    public override Vector2 Calculate(MoveAbstract target)
    {
        Vector2 desired = Direction(target);
        var speed = me.maxSpeed;

        if (desired.sqrMagnitude <= (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
            speed = -me.maxSpeed * ((desired.magnitude - 1) / me._desaceleration.current);

        desired *= speed;

        Vector2 st = desired - me.vectorVelocity;
        st = Vector2.ClampMagnitude(st, me.maxSpeed);

        return st;


        //original
        //_desiredVelocity = Vector2.ClampMagnitude(target.transform.position, me.maxSpeed);

        //if (_desiredVelocity.sqrMagnitude < (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
        //    _desiredVelocity = -me.vectorVelocity * (me._desaceleration.current - 1);

        //_steering = _desiredVelocity - me.vectorVelocity;

        //var vecVelocity = me.vectorVelocity;

        //vecVelocity += _steering * Time.deltaTime;

        //return vecVelocity;

        //Vector2 desired = Direction(target);


        //ver arreglada version1
        //Vector2 desired = Direction(target);
        //if (desired.sqrMagnitude < (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
        //    desired = -me.vectorVelocity * (me._desaceleration.current - 1);

        //_steering = desired - me.vectorVelocity;

        //var vecVelocity = me.vectorVelocity;
        //vecVelocity += _steering;

        //vecVelocity = Vector2.ClampMagnitude(vecVelocity, me.maxSpeed);

        //return vecVelocity;
    }
}
