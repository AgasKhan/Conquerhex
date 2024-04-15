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
        MyStarts += MyStart;
    }

    private void MyAwake()
    {
        health.death += () => gameObject.SetActive(false);
    }

    private void MyStart()
    {
        hexagoneParent = GetComponentInParent<Hexagone>();
    }
}
