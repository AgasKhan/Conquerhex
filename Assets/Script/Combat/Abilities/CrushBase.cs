using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Crush")]
public class CrushBase : AreaKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeRangeUpWeaponKata);
    }
}