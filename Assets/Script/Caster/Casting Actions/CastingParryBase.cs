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
        return typeof(CastingDash);
    }
}


public class CastingParry : CastingAction<CastingParryBase>
{
    Timer parryTime;

    CastingAction successCastingAction;

    CastingAction failureCastingAction;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        parryTime = TimersManager.Create(castingActionBase.parryTime, Update, Finish).Stop();

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

        ability.caster.container.health.lifeUpdate += Parry;
    }


    bool successParry = false;
    private void Parry(IGetPercentage arg1, float arg2)
    {
        successParry = true;
    }

    void Update()
    {
        if(successParry)
        {
            ability.ApplyCast(successCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);
        }

        if (End)
        {
            ability.ApplyCast(failureCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);
        }
    }

    void Finish()
    {

    }

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        throw new NotImplementedException();
    }
}