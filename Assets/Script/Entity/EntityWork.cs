using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWork : MyScripts
{
    [SerializeField]
    FSMWork fsmWork;

    [SerializeField]
    InventoryEntityComponent staticEntity;
    protected override void Config()
    {

        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
    }

    private void MyAwake()
    {
        fsmWork.Init(staticEntity);
    }

    private void MyUpdate()
    {
        fsmWork.UpdateState();
    }
}