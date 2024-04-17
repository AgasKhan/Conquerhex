using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpTriggerControllerBase")]
public class ChargeRangeUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Multiplicador de rango que se aplica por tiempo")]
    public float multiplyRange = 1;
    protected override System.Type SetItemType()
    {
        return typeof(ChargeRangeUpTrggrCtrllr);
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeRangeUpTrggrCtrllr : UpTrggrCtrllr
{
    new public ChargeRangeUpTrggrCtrllrBase triggerBase => (ChargeRangeUpTrggrCtrllrBase)base.triggerBase;

    public override float FinalMaxRange => Mathf.Clamp(range * triggerBase.multiplyRange, 1, base.FinalMaxRange);

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