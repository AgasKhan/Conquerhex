using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class StabBase : AreaKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(DashUpWeaponKata);
    }
}