using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MultiCut")]
public class MultiCutBase : AreaKataBase
{
    protected override void SetCreateItemType()
    {
        _itemType = typeof(UpWeaponKata);
    }
}