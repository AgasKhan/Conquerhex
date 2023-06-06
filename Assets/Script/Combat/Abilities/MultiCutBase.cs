using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MultiCut")]
public class MultiCutBase : AreaKataBase
{
    [Tooltip("Multiplicador de espera para el golpe automatico")]
    public float timeToAttackPress;

    public override Item Create()
    {
        PressWeaponKata aux = base.Create() as PressWeaponKata;
        aux.pressCooldown = TimersManager.Create(timeToAttackPress*velocity);

        return aux;
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(PressWeaponKata);
    }
}