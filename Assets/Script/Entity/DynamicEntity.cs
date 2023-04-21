using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DinamicEntity : StaticEntity
{
    public MoveAbstract move;
}

public abstract class DinamicEntityWork : DinamicEntity
{
    [SerializeField]
    FSMWork fsmWork;

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
    }

    private void MyAwake()
    {
        fsmWork.Init(this);
    }

    private void MyUpdate()
    {
        fsmWork.UpdateState();
    }
}
