using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpWeaponKataBase")]
public class ChargeRangeUpWeaponKataBase : WeaponKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeRangeUpWeaponKata);
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeRangeUpWeaponKata : UpWeaponKata
{
    public override float FinalRange => Mathf.Clamp(range * itemBase.velocityCharge, 1, base.FinalRange);

    float range;

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        range = 0;
        base.InternalControllerDown(dir, button);
    }

    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        range = button;
        base.InternalControllerPress(dir, button);
    }
}