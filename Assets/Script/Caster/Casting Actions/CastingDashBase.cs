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
    Timer dashInTime;

    MoveEntityComponent moveEntity;

    CastingAction startDashCastingAction;

    CastingAction updateDashCastingAction;

    CastingAction endDashCastingAction;

    bool stopUpdateCast = false;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        dashInTime = TimersManager.Create(castingActionBase.dashInTime, Update , Finish).Stop();

        if(castingActionBase.startDashCastingAction!=null)
        {
            startDashCastingAction = castingActionBase.startDashCastingAction.Create();
            startDashCastingAction.Init(ability);
        }

        if(castingActionBase.updateDashCastingAction != null)
        {
            updateDashCastingAction = castingActionBase.updateDashCastingAction.Create();
            updateDashCastingAction.Init(ability);
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

        IEnumerable<Entity> affected = null;

        if (caster.TryGetInContainer(out moveEntity))
        {
            dashInTime.Reset();
            affected = startDashCastingAction?.InternalCastOfExternalCasting(ability.Detect(), out showParticleInPos, out showParticleDamaged);
        }

        if (updateDashCastingAction != null)
        {
            stopUpdateCast = false;
            GameManager.instance.StartCoroutine(ApplyCastUpdate());
        }

        return affected;
    }

    IEnumerator ApplyCastUpdate()
    {
        ability.ApplyCast(updateDashCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged));
        yield return new WaitForSeconds(0.2f);

        if(!stopUpdateCast)
            yield return GameManager.instance.StartCoroutine(ApplyCastUpdate());
    }

    void Update()
    {
        if (castingActionBase.multiplyByArea)
        {
            moveEntity.Velocity(VirtualControllers.Movement.dir.normalized.Vect2To3XZ(0), castingActionBase.velocityInDash * FinalMaxRange);
        }
        else
        {
            moveEntity.Velocity(VirtualControllers.Movement.dir.normalized.Vect2To3XZ(0), castingActionBase.velocityInDash);
        }
    }

    void Finish()
    {
        moveEntity.Velocity(moveEntity.direction, moveEntity.objectiveVelocity);

        if(endDashCastingAction!=null)
            ability.ApplyCast(endDashCastingAction.InternalCastOfExternalCasting(ability.Detect(), out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);
        
        stopUpdateCast = true;
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
