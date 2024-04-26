using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/CastingWeaponKataInSlot", fileName = "new CastingWeaponKataInSlot")]
public class CastingWeaponKataInSlotBase : CastingActionBase
{
    protected override Type SetItemType()
    {
        return typeof(CastingWeaponKataInSlot);
    }
}

public class CastingWeaponKataInSlot : CastingAction
{
    int slot;
    public override void Init(Ability ability)
    {
        base.Init(ability);
        ability.OnEquipedInSlot += AbilityOnEquipedInSlot;
    }

    public override void Destroy()
    {
        ability.OnEquipedInSlot -= AbilityOnEquipedInSlot;
        base.Destroy();
    }

    public override IEnumerable<Entity> Cast(List<Entity> entities)
    {
        End = true;

        if(caster.katasCombo[slot]?.equiped != null)
        {
            caster.katasCombo[slot].equiped.Detect();

            return caster.katasCombo[slot].equiped.Cast();
        }

        return entities;
    }

    private void AbilityOnEquipedInSlot(int obj)
    {
        if (obj >= 2)
            slot = obj - 2;
        else
            slot = 0;
    }
}

