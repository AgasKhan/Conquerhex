using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Explocion")]

public class ExplocionBase : AreaKataBase
{
    protected override void SetCreateItemType()
    {
        _itemType = typeof(UpWeaponKata);
    }
}

