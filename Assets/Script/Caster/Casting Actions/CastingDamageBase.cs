using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/CastingDamageBase", fileName = "new CastingDamageBase")]
public class CastingDamageBase : CastingActionBase
{
    public Damage[] damages;

    protected override Type SetItemType()
    {
        return typeof(CastingDamage);
    }
}

public class CastingDamage : CastingAction<CastingDamageBase>
{
    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities)
    {
        var additiveDamage = Damage.Combine(Damage.AdditiveFusion, castingActionBase.damages, caster.additiveDamage.content);

        var multiplative = Damage.Combine(Damage.MultiplicativeFusion, ability.multiplyDamage.content, additiveDamage);

        return Damage.ApplyDamage(caster.container,multiplative, entities); 
    }
}
