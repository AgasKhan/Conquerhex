using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Explocion")]

public class ExplocionBase : AreaKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(UpWeaponKata);
    }
}

