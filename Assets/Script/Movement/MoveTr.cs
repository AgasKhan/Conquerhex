using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTr : MoveAbstract
{
    protected override void Config()
    {
        MyUpdates += MyUpdate;
    }

    public void MyUpdate()
    {
        transform.position += (direction * _frameVelocity).Vec2to3(0);

        _velocity.Substract(_frameDesaceleration);
    }

}
