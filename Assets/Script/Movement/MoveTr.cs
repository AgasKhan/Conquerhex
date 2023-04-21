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
        transform.position += (direction * _velocity.current * Time.deltaTime).Vec2to3(0);

        _velocity.Substract(_desaceleration.current * Time.deltaTime);
    }

}
