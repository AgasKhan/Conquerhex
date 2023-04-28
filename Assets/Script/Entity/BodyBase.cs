using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/body", fileName = "new Body")]
public class BodyBase : ItemBase
{
    [Header("Estadisticas")]
    public float life;

    public float regen;

    public float velocity;

    public float weightCapacity;

    public float stunTime;

    [Header("Ataque")]

    public WeaponKataCombo principal;

    public WeaponKataCombo secondary;

    public WeaponKataCombo tertiary;

    [Header("Defensa")]

    public Damage[] vulnerabilities;

    [Header("Especialization")]

    public Damage[] additiveDamage;


    protected override void SetCreateItemType()
    {
        _itemType = typeof(Body);
    }
}



public class Body : Item<BodyBase>
{
    public override void Init(params object[] param)
    {
        
    }
}

[System.Serializable]
public class WeaponKataCombo
{
    public WeaponBase weapon;

    public WeaponKataBase kata;
}