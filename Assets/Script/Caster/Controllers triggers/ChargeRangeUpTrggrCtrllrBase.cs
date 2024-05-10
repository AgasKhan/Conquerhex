using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpTriggerControllerBase")]
public class ChargeRangeUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("El tiempo que tardara en cargar la habilidad para tener el rango maximo")]
    public float timeToCompleteRange = 1;

    [Tooltip("Rango minimo que tendra")]
    public float minimalRange = 1;
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

    public override float FinalMaxRange => Mathf.Clamp((range / triggerBase.timeToCompleteRange) * base.FinalMaxRange, triggerBase.minimalRange, base.FinalMaxRange);

    float range;

    public override void ControllerDown(Vector2 dir, float button)
    {
        range = 0;
        base.ControllerDown(dir, button);
    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        FeedBackReference?.DotAngle(Dot);

        range = button;
        base.ControllerPressed(dir, button);
    }
}