using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTr : MoveAbstract
{

    private void Update()
    {
        transform.position += (direction * _frameVelocity).Vec2to3(0);

        _velocity.Substract(_frameDesaceleration);
    }

}
