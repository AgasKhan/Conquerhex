using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : InventoryEntityComponent
{
    [SerializeField]
    SpriteRenderer mySprite;

    ResourcesBase_ItemBase itemBase;

    public float weight => itemBase.weight;

    Timer recolect;

    InventoryEntityComponent referenceToTravel;

    Hexagone hex;

    void Awake()
    {
        recolect = TimersManager.Create(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 1, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            referenceToTravel.AddAllItems(this);
            gameObject.SetActive(false);
            
        })
        .Stop().SetInitCurrent(0);

    }

    void FixedUpdate()
    {
        foreach (var entity in hex.childsEntities)
        {
            if (!(entity is Character))
                continue;

            if (transform.IsInRadius(entity, entity.flyweight.areaFarming))
            {
                Recolect(entity.GetInContainer<InventoryEntityComponent>());
                break;
            }
        }   
    }

    public void Recolect(InventoryEntityComponent entity)
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
        //health.Init(item.structure.life, item.structure.regen);

        mySprite.sprite = item.image;

        AddOrSubstractItems(item.nameDisplay, 1);

        itemBase = item;


        hex = GetComponentInParent<Hexagone>();
    }
}


