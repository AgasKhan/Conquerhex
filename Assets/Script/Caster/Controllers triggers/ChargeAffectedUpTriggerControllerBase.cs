using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeAffectedUpTriggerControllerBase")]
public class ChargeAffectedUpTriggerControllerBase : TriggerControllerBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeAffectedUpTriggerController);
    }
}

public class ChargeAffectedUpTriggerController : UpTriggerController
{
    public override List<Entity> Detect(Vector2 dir, float timePressed = 0, float? range = null, float? dot = null)
    {
        return ability.itemBase.Detect(ref ability.affected, caster.container, dir, (int)Mathf.Clamp(timePressed * ability.itemBase.velocityCharge, 1, ability.itemBase.maxDetects), FinalRange, dot ?? ability.itemBase.dot);
    }
}