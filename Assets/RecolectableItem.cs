using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : StaticEntity
{
    [SerializeField]
    SpriteRenderer mySprite;

    StructureBase myStructureBase;

    public float weight;

    protected override Damage[] vulnerabilities => myStructureBase.vulnerabilities;

    Timer recolect;

    StaticEntity referenceToTravel;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        recolect = TimersManager.LerpInTime(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 1, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            
        })
        .Stop();

        recolect.current = 0;
    }

    public void Recolect(StaticEntity entity)
    {
        if (!recolect.Chck)
            return;

        //Debug.Log("me quiere recoger: " + entity.name);

        referenceToTravel = entity;

        entity.travelItem.Add(recolect);

        recolect.Reset();
    }

    public void CopyFrom(RecolectableItem other)
    {
        AddAllItems(other.inventory);
    }


    public void Init(ResourcesBase_ItemBase item)
    {
        health.Init(item.structure.life, item.structure.regen);

        mySprite.sprite = item.image;

        AddOrSubstractItems(item.nameDisplay, 1);

        weight = item.weight;
    }
}


