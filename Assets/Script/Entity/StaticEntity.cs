using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticEntity : Entity
{
    public Pictionarys<string,LogicActive> interact; //funciones de un uso para la interaccion

    public List<Item> inventory;

    [SerializeField]
    Pictionarys<string, LogicActive> actions; //funciones de un uso para cuestiones internas
}


public abstract class StaticEntityWork : StaticEntity
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

