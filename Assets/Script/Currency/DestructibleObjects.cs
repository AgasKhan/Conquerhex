using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryEntityComponent))]
[RequireComponent(typeof(DropEntityComponent))]
public class DestructibleObjects : Entity
{
    //public EntityBase _structure;
    public InventoryEntityComponent inventory;
    public DropEntityComponent drop;
    
    //protected override Damage[] vulnerabilities => _structure.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        health.death += () => TimersManager.Create(0.2f, () => gameObject.SetActive(false)).Reset();
    }
}
