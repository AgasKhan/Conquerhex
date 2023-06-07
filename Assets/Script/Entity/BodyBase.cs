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

    [Header("Ataque")]

    public WeaponKataCombo principal;

    public WeaponKataCombo secondary;

    public WeaponKataCombo tertiary;

    [Header("Especialization")]

    public Damage[] additiveDamage;


    protected override void SetCreateItemType()
    {
        _itemType = typeof(BodyDiagram);
    }
}



public class BodyDiagram : Item<BodyBase>
{
    public override void Init(params object[] param)
    {
        
    }
}

[System.Serializable]
public class WeaponKataCombo
{
    public MeleeWeaponBase weapon;

    public WeaponKataBase kata;
}