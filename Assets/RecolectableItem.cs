using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : StaticEntity
{
    [SerializeField]
    ItemBase initialItems;
    [SerializeField]
    int amount;

    [SerializeField]
    StructureBase structureBase;
    protected override Damage[] vulnerabilities => structureBase.vulnerabilities;

    Timer recolect;

    StaticEntity referenceToTravel;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        health.Init(structureBase.life, structureBase.regen);

        recolect = TimersManager.LerpInTime(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 1, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            
        })
        .Stop();

        recolect.current = 0;

        AddOrSubstractItems(initialItems.nameDisplay, amount);
    }

    public void Recolect(StaticEntity entity)
    {
        if (!recolect.Chck)
            return;

        //Debug.Log("me quiere recoger: " + entity.name);

        referenceToTravel = entity;

        recolect.Reset();
    }

    public void CopyFrom(RecolectableItem other)
    {
        AddAllItems(other.inventory);
    }
}


