using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/CastingExponentialDamageBase", fileName = "newCastingExponentialDamage")]
public class CastingExponentialDamageBase: CastingActionBase
{
    public Damage[] damagesMultiplier;
    protected override Type SetItemType()
    {
        return typeof(CastingExponentialDamage);
    }
}

public class CastingExponentialDamage : CastingAction<CastingExponentialDamageBase>
{
    int castTimes;
    float energyCost;
    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = true;
        showParticleDamaged = true;

        IEnumerable<Damage> multiplative = default;

        if (ability.PayExecution(energyCost) || End == true)
        {
            var additiveDamage = Damage.Combine(Damage.AdditiveFusion, castingActionBase.damagesMultiplier, caster.additiveDamage.content);

            for (int i = 0; i < castTimes; i++)
            {
                additiveDamage = Damage.Combine(Damage.AdditiveFusion, additiveDamage, additiveDamage);
            }

            multiplative = Damage.Combine(Damage.MultiplicativeFusion, ability.multiplyDamage.content, additiveDamage);

            End = true;
            castTimes = 0;
            energyCost = ability.CostExecution;
            return Damage.ApplyDamage(caster.container, multiplative, entities);
        }
        else
        {
            energyCost *= 2;
            castTimes++;
            Debug.Log("ENERGY COST: " + energyCost + "\nCAST TIMES: " + castTimes);
        }

        //End = true;
        Debug.Log("NO PASO POR APPLY DAMAGE");
        return Damage.ApplyDamage(caster.container, default, entities);
    }

    public override void Init(Ability ability)
    {
        base.Init(ability);
        castTimes = 0;
        energyCost = ability.CostExecution;
    }
}
