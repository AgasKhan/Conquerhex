using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/CastingPArry", fileName = "new CastingParry")]
public class CastingParryBase : CastingActionBase
{
    [Tooltip("El tiempo que tiene el jugador antes de que el parry se considere fallido")]
    public float parryTime = 2;

    public CastingActionBase successCastingAction;

    public CastingActionBase failureCastingAction;

    protected override Type SetItemType()
    {
        return typeof(CastingParry);
    }
}


public class CastingParry : CastingAction<CastingParryBase>
{
    Timer parryTime;

    CastingAction successCastingAction;

    CastingAction failureCastingAction;

    bool successParry = false;

    System.Action<(Damage dmg, int weightAction, Vector3? origin)> takeDamage;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        parryTime = TimersManager.Create(castingActionBase.parryTime, Finish).Stop();

        if(castingActionBase.successCastingAction != null)
        {
            successCastingAction = castingActionBase.successCastingAction.Create();
            successCastingAction.Init(ability);
        }
        if (castingActionBase.failureCastingAction != null)
        {
            failureCastingAction = castingActionBase.failureCastingAction.Create();
            failureCastingAction.Init(ability);
        }

        caster.onTakeDamage += TriggerTakeDamage;
    }

    public override void Destroy()
    {
        caster.onTakeDamage -= TriggerTakeDamage;
        base.Destroy();
    }

    private void TriggerTakeDamage((Damage dmg, int weightAction, Vector3? origin) obj)
    {
        takeDamage?.Invoke(obj);
    }

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = false;
        showParticleDamaged = false;

        caster.container.vulnerabilities.Add(Damage.Create<DamageTypes.PureDamage>(0, 0, "Parry"));

        takeDamage = OnTakeDamage;

        parryTime.Reset();

        End = false;

        successParry = false;

        return entities;
    }

    private void OnTakeDamage((Damage dmg, int weightAction, Vector3? origin) obj)
    {
        if (obj.dmg.typeInstance.IsParent)
            return;

        successParry = true;
        if (successCastingAction != null)
        { 
            ability.ApplyCast(successCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool shPos, out bool shDmg), shPos, shDmg);
            ability.PlaySound("Succes");
        }
    }

    void Finish()
    {
        takeDamage = null;

        caster.container.vulnerabilities.Remove(Damage.Create<DamageTypes.PureDamage>(0, 0, "Parry"));

        if (!successParry)
        {
            if(failureCastingAction!=null)
                ability.ApplyCast(failureCastingAction.InternalCastOfExternalCasting(null, out bool shPos, out bool shDmg), shPos, shDmg);

            caster.positiveEnergy = 0;
            ability.PlaySound("Fail");
        }
        else
        {
            caster.positiveEnergy = 75;
        }
                       
        End = true;
    }
}