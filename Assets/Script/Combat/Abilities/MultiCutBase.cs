using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MultiCut")]
public class MultiCutBase : AreaKataBase
{
    public float timeToAttackPress;

    public override Item Create()
    {
        PressWeaponKata aux = base.Create() as PressWeaponKata;
        aux.pressCooldown = TimersManager.Create(timeToAttackPress);

        return aux;
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(PressWeaponKata);
    }
}