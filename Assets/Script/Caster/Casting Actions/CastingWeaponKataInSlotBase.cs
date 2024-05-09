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

    public override bool DontExecuteCast => base.DontExecuteCast || caster.katasCombo[slot]?.equiped == null;

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

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        End = true;

        if(caster.katasCombo[slot]?.equiped != null)
        {
            //caster.katasCombo[slot].equiped.Detect();

            return caster.katasCombo[slot].equiped.Cast(entities, out showParticleInPos, out showParticleDamaged);
        }

        showParticleInPos = false;
        showParticleDamaged = false;

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

