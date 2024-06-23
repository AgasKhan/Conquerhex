using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityModificators;

[CreateAssetMenu(menuName = "Abilities/ChargeRangeUpTriggerControllerBase")]
public class ChargeRangeUpTrggrCtrllrBase : ModificatorBase
{
    [Tooltip("Rango minimo que tendra")]
    public float minimalRange = 1;

    [Tooltip("multiplica la velocidad de crecimiento")]
    public float timeMultiply = 1;

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
    public override float FinalMaxRange => abilityModifier.FinalMaxRange == 0 ? original.FinalMaxRange : Mathf.Clamp((range / abilityModifier.FinalMaxRange) * base.FinalMaxRange, modificatorBase.minimalRange, base.FinalMaxRange);

    float range;

    protected override float Operation(float previusValue, float flyweightValue)
    {
        if(operationType==OperationType.multiply)
            return Mathf.Clamp((range / flyweightValue) * previusValue, 0, previusValue);
        else
            return Mathf.Lerp(flyweightValue, previusValue, range);
    }

    public override void ControllerDown(Vector2 dir, float button)
    {
        range = 0;
    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        range = button * modificatorBase.timeMultiply;
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
        //range = 0;
    }


}