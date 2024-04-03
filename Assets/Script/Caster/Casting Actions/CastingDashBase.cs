using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/CastingDash", fileName = "new CastingDash")]
public class CastingDashBase : CastingActionBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash = 10;

    protected override Type SetItemType()
    {
        return typeof(CastingDash);
    }
}

public class CastingDash : CastingAction<CastingDashBase>
{
    public override IEnumerable<Entity> Cast(List<Entity> entities)
    {
        if (caster.TryGetInContainer<MoveEntityComponent>(out var aux))
        {
            aux.move.Velocity(aux.move.direction * castingActionBase.velocityInDash);
        }

        return null;
    }
}
