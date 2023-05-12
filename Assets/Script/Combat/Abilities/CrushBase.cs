using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Crush")]
public class CrushBase : AreaKataBase
{
    protected override void SetCreateItemType()
    {
        _itemType = typeof(UpWeaponKata);
    }
}