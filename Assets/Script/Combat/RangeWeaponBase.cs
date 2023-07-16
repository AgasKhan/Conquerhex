using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Range", fileName = "New weapons")]
public class RangeWeaponBase : MeleeWeaponBase
{
    [Header("Ranged")]
    public int magazine;

    protected override void SetCreateItemType()
    {
        _itemType = typeof(RangeWeapon);
    }
}


public class RangeWeapon : MeleeWeapon
{
    public Tim amunation;

    public GameObject prefabBullet;

    public override void Init(params object[] param)
    {
        base.Init(param);

        amunation = new Tim(((RangeWeaponBase)itemBase).magazine);
    }

    public override void Durability(float damageToDurability)
    {
        if (amunation.Substract(1) <= 0)
        {
            TriggerOff();
            return;
        }

        base.Durability(damageToDurability);
    }
}
