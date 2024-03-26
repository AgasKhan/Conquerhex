using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeAffectedUpWeaponKataBase")]
public class ChargeAffectedUpWeaponKataBase : WeaponKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeAffectedUpWeaponKata);
    }
}

public class ChargeAffectedUpWeaponKata : UpWeaponKata
{
    protected override List<Entity> InternalDetect(Vector2 dir, float timePressed = 0, float? range = null, float? dot = null)
    {
        return itemBase.Detect(ref affected, caster.container, dir, (int)Mathf.Clamp(timePressed * itemBase.velocityCharge, 1, itemBase.maxDetects), FinalRange, dot ?? itemBase.dot);
    }
}