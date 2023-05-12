using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cut")]
public class CutBase : AreaKataBase
{
    protected override void SetCreateItemType()
    {
        _itemType = typeof(UpWeaponKata);
    }
}
