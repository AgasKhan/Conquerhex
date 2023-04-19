using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DinamicEntity : StaticEntity
{
    public MoveAbstract move;
}

public abstract class DinamicEntityWork : DinamicEntity
{
    [SerializeReference]
    FSMWork fsmWork;

    private void Awake()
    {
        fsmWork.Init(this);
    }

    private void Update()
    {
        fsmWork.UpdateState();
    }
}
