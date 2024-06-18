using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/CastingDash", fileName = "new CastingDash")]
public class CastingDashBase : CastingActionBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash = 10;

    [Tooltip("Tiempo en la que se ejercera la velocidad")]
    public float dashInTime = 1f;

    [Tooltip("multiplica la velocidad del dash por el tamanio del area")]
    public bool multiplyByArea = false;

    public CastingActionBase startDashCastingAction;

    public CastingActionBase updateDashCastingAction;

    public CastingActionBase endDashCastingAction;

    protected override Type SetItemType()
    {
        return typeof(CastingDash);
    }
}

public class CastingDash : CastingAction<CastingDashBase>
{
    TimedCompleteAction dashInTime;

    Timer timerToCastUpdate;

    MoveEntityComponent moveEntity;

    CastingAction startDashCastingAction;

    CastingAction updateDashCastingAction;

    CastingAction endDashCastingAction;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        dashInTime = TimersManager.Create(castingActionBase.dashInTime, Update , Finish);
        dashInTime.Stop();

        if(castingActionBase.startDashCastingAction!=null)
        {
            startDashCastingAction = castingActionBase.startDashCastingAction.Create();
            startDashCastingAction.Init(ability);
        }

        if(castingActionBase.updateDashCastingAction != null)
        {
            updateDashCastingAction = castingActionBase.updateDashCastingAction.Create();
            updateDashCastingAction.Init(ability);
            timerToCastUpdate = TimersManager.Create(0.2f, ApplyCastUpdate).SetLoop(true).Stop();
        }

        if(castingActionBase.endDashCastingAction!=null)
        {
            endDashCastingAction = castingActionBase.endDashCastingAction.Create();
            endDashCastingAction.Init(ability);
        }        
    }

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = false;
        showParticleDamaged = false;

        IEnumerable<Entity> affected = Utilitys.VoidEnumerable<Entity>();

        if (caster.TryGetInContainer(out moveEntity))
        {
            dashInTime.Reset();
            affected = startDashCastingAction?.InternalCastOfExternalCasting(ability.Detect(), out showParticleInPos, out showParticleDamaged);
        }

        //ability.state = Ability.State.middle;
        End = false;

        if (updateDashCastingAction != null)
        {
            timerToCastUpdate.Reset();
        }

        return affected;
    }

    void ApplyCastUpdate()
    {
        ability.ApplyCast(updateDashCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged));
    }

    void Update()
    {
        if (castingActionBase.multiplyByArea)
        {
            moveEntity.Velocity(Aiming, castingActionBase.velocityInDash * FinalMaxRange);

            //Debug.Log("Velocity: " + FinalMaxRange);
        }
        else
        {
            moveEntity.Velocity(Aiming, castingActionBase.velocityInDash);
        }
    }

    void Finish()
    {
        timerToCastUpdate?.Stop();

        moveEntity.Velocity(moveEntity.direction, moveEntity.objectiveVelocity);

        if(endDashCastingAction!=null)
            ability.ApplyCast(endDashCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);

        End = true;
    }

    public override void Destroy()
    {
        dashInTime?.Stop();
        dashInTime = null;
        startDashCastingAction?.Destroy();
        updateDashCastingAction?.Destroy();
        endDashCastingAction?.Destroy();

        base.Destroy();
    }
}
