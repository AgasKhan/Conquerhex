using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/CastingExponentialDamageBase", fileName = "new CastingExponentialDamageBase")]
public class CastingExponentialDamageBase: CastingActionBase
{
    public float finalDamageMultiplier;

    public Damage[] damages;
    protected override Type SetItemType()
    {
        return typeof(CastingExponentialDamage);
    }
}

public class CastingExponentialDamage : CastingAction<CastingExponentialDamageBase>
{
    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = true;
        showParticleDamaged = true;


        var additiveDamage = Damage.Combine(Damage.AdditiveFusion, castingActionBase.damages, caster.additiveDamage.content);

        var multiplative = Damage.Combine(Damage.MultiplicativeFusion, ability.multiplyDamage.content, additiveDamage);

        End = true;

        return Damage.ApplyDamage(caster.container, multiplative, entities);
    }
}
