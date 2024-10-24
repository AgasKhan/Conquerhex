using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : StaticEntity
{
    [SerializeField]
    SpriteRenderer mySprite;

    [SerializeField]
    protected Detect<Character> areaFarming;

    ResourcesBase_ItemBase itemBase;

    public float weight => itemBase.weight;

    protected override Damage[] vulnerabilities => null;

    Timer recolect;

    StaticEntity referenceToTravel;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
    }

    void MyAwake()
    {
        recolect = TimersManager.Create(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 1, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            
        })
        .Stop().SetInitCurrent(0);
    }

    void MyUpdate()
    {
        var characters = areaFarming.Area(transform.position, (algo) => { return true; });

        foreach (var character in characters)
        {
            //if (character.currentWeight + weight <= character.weightCapacity)
            var aux = (BodyBase)character.flyweight;
            var dist = character.transform.position - transform.position;

            if (dist.sqrMagnitude <= aux.areaFarming * aux.areaFarming && character == GameManager.instance.playerCharacter)
            {
                Recolect(character);
                break;
            }
        }
    }

    public void Recolect(StaticEntity entity)
    {
        if (!recolect.Chck /* && (entity.currentWeight + weight) <= entity.weightCapacity*/)
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

        itemBase = item;
    }
}


