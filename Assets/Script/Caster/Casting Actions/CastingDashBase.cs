using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/CastingDash", fileName = "new CastingDash")]
public class CastingDashBase : CastingActionBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash = 10;

    [Tooltip("Tiempo en la que se ejercera la velocidad")]
    public float dashInTime = 1f;

    public CastingActionBase startDashCastingAction;

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

    CastingAction endDashCastingAction;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        dashInTime = TimersManager.Create(castingActionBase.dashInTime, Update , Finish).Stop();

        if(castingActionBase.startDashCastingAction!=null)
        {
            startDashCastingAction = castingActionBase.startDashCastingAction.Create();
            startDashCastingAction.Init(ability);
        }

        if(castingActionBase.endDashCastingAction!=null)
        {
            endDashCastingAction = castingActionBase.endDashCastingAction.Create();
            endDashCastingAction.Init(ability);
        }        
    }

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities)
    {
        IEnumerable<Entity> affected = null;

        if (caster.TryGetInContainer(out moveEntity))
        {
            dashInTime.Reset();
            affected = startDashCastingAction?.InternalCastOfExternalCasting(ability.Detect());
        }

        return affected;
    }

    void Update()
    {
        moveEntity.Velocity(Aiming, castingActionBase.velocityInDash);
    }

    void Finish()
    {
        moveEntity.Velocity(moveEntity.direction, moveEntity.objectiveVelocity);

        ability.ApplyCast(endDashCastingAction?.InternalCastOfExternalCasting(ability.Detect()));

        End = true;
    }

    public override void Destroy()
    {
        dashInTime?.Stop();
        dashInTime = null;
        startDashCastingAction?.Destroy();
        endDashCastingAction?.Destroy();

        base.Destroy();
    }
}
