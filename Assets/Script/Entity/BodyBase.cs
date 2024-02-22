using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/body", fileName = "new Body")]
public class BodyBase : StructureBase
{
    [Header("Estadisiticas")]

    public float velocity;

    public float weightCapacity;

    public float stunTime;

    public float areaFarming=1;


    protected override void SetCreateItemType()
    {
        _itemType = typeof(BodyDiagram);
    }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Velocity ", velocity.ToString());
        aux.Add("Weight Capacity ", weightCapacity.ToString());
        aux.Add("Area Farming ", areaFarming.ToString());

        return aux;
    }
}



public class BodyDiagram : Item<BodyBase>
{
    public override void Init()
    {
        
    }
}

[System.Serializable]
public class WeaponKataCombo
{
    public WeaponKataBase kata;

    public MeleeWeaponBase weapon;
}