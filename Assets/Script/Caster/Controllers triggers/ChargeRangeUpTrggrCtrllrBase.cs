using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpTriggerControllerBase")]
public class ChargeRangeUpTrggrCtrllrBase : ModificatorBase
{
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
public class ChargeRangeUpTrggrCtrllr : Modificator<ChargeRangeUpTrggrCtrllrBase>
{

    public override float Angle => abilityModifier.Angle== 0 ? base.Angle : Mathf.Clamp((range / abilityModifier.Angle) * base.Angle, 0, base.Angle);

    public override float FinalMinRange => abilityModifier.FinalMinRange == 0 ? base.FinalMinRange : Mathf.Clamp((range / abilityModifier.FinalMinRange) * base.FinalMinRange, 0, base.FinalMaxRange);

    public override float FinalMaxRange => abilityModifier.FinalMaxRange == 0 ? base.FinalMaxRange : Mathf.Clamp((range / abilityModifier.FinalMaxRange) * base.FinalMaxRange, modificatorBase.minimalRange, base.FinalMaxRange);

    public override int FinalMaxDetects => abilityModifier.FinalMaxDetects == 0 ? base.FinalMaxDetects : (int)Mathf.Clamp((range / abilityModifier.FinalMaxDetects) * base.FinalMaxDetects, 0, base.FinalMaxDetects);

    public override float Auxiliar => abilityModifier.Auxiliar == 0 ? base.Auxiliar : Mathf.Clamp((range / abilityModifier.Auxiliar) * base.Auxiliar, 0, base.Auxiliar);

    float range;

    public override void ControllerDown(Vector2 dir, float button)
    {
        range = 0;
    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        range = button;
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
        range = 0;
    }
}