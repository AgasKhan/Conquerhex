using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cut")]
public class CutBase : AreaKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeAffectedUpWeaponKata);
    }
}
