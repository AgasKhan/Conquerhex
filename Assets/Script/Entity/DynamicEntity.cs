using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicEntity : AttackEntity
{
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        move.onTeleport += Teleport;
    }

    public MoveAbstract move;
}

