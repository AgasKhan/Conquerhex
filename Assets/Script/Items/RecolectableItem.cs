using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableItem : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer mySprite;

    ItemBase myItemBase;

    public float weight => myItemBase.weight;

    Timer recolect;
    Timer wait;

    public InventoryEntityComponent inventory;

    AudioEntityComponent audio;
    InventoryEntityComponent referenceToTravel;

    Hexagone hex;

    void Awake()
    {
        audio = GetComponent<AudioEntityComponent>();
        recolect = TimersManager.Create(() => transform.position, ()=> referenceToTravel.transform.position + Vector3.up, 0.85f, Vector3.Slerp, (pos) => transform.position = pos)
        .AddToEnd(() =>
        {
            audio.Play("RecolectAudio");
            referenceToTravel.AddItem(myItemBase,1);
            UI.Interfaz.instance?["Danio"].AddMsg((myItemBase.nameDisplay + " x " + 1).RichTextColor(SetColorTxt(((ResourcesBase_ItemBase)myItemBase).rarity)));
            transform.SetActiveGameObject(false);
        })
        .Stop().SetInitCurrent(0);

        wait = TimersManager.Create(0.3f).Stop();
    }

    void FixedUpdate()
    {
        if (hex == null || !wait.Chck)
            return;

        foreach (var entity in hex.childsEntities)
        {
            if (!(entity is Character) || entity.health.IsDeath)
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
    }

    private void ChangeDisponiblity(InventoryEntityComponent obj)
    {
        if(!referenceToTravel.HasCapacity(1, myItemBase))
        {
            recolect.Stop().SetInitCurrent(0);
            referenceToTravel.onChangeDisponiblity -= ChangeDisponiblity;
        }
    }

    Color SetColorTxt(GachaRarity rarity)
    {
        switch(rarity)
        {
            case GachaRarity.C:
                return Color.white;
            case GachaRarity.B:
                return Color.green;
            case GachaRarity.A:
                return Color.yellow;
            default:
                return Color.gray;
        }
    }

    public void Init(ItemBase item)
    {
        mySprite.sprite = item.image;

        myItemBase = item;

        wait.Reset();

        hex = GetComponentInParent<Hexagone>();
    }
}


