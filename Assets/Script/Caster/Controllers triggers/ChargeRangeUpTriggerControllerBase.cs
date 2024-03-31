using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpTriggerControllerBase")]
public class ChargeRangeUpTriggerControllerBase : TriggerControllerBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeRangeUpTriggerController);
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeRangeUpTriggerController : UpTriggerController
{
    public override float FinalRange => Mathf.Clamp(range * ability.itemBase.velocityCharge, 1, base.FinalRange);

    float range;

    public override void ControllerDown(Vector2 dir, float button)
    {
        range = 0;
        base.ControllerDown(dir, button);
    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        range = button;
        base.ControllerPressed(dir, button);
    }
}