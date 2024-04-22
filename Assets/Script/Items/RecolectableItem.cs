using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer mySprite;

    ItemBase myItemBase;
    Item myItem;

    public float weight => myItemBase.weight;

    Timer recolect;

    public InventoryEntityComponent inventory;
    InventoryEntityComponent referenceToTravel;

    Hexagone hex;

    void Awake()
    {
        recolect = TimersManager.Create(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 0.85f, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            myItem.Init(referenceToTravel);
            transform.gameObject.SetActive(false);
            Debug.Log("------------------------------------------\nItem Recogido");
        })
        .Stop().SetInitCurrent(0);
    }

    void FixedUpdate()
    {
        if (hex == null)
            return;

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
        referenceToTravel.onChangeDisponiblity += ChangeDisponiblity;
        recolect.Reset();
        myItem = myItemBase.Create();
    }

    private void ChangeDisponiblity(InventoryEntityComponent obj)
    {
        if(referenceToTravel.HasCapacity(myItem))
        {
            recolect.Stop().SetInitCurrent(0);
            referenceToTravel.onChangeDisponiblity -= ChangeDisponiblity;
        }
    }

    public void Init(ItemBase item)
    {
        mySprite.sprite = item.image;

        myItemBase = item;
        hex = GetComponentInParent<Hexagone>();
    }
}


