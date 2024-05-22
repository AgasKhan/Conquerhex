using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityModificators;

public class ChargeStatsPressedTrggrCtrllrBase : ModificatorBase
{
    protected override System.Type SetItemType()
    {
        return typeof(ChargeStatsPressedTrggrCtrllr);
    }
}

public class ChargeStatsPressedTrggrCtrllr : Modificator<ChargeStatsPressedTrggrCtrllrBase>
{
    public override float Angle => abilityModifier.Angle == 0 || !cntrllBool ? base.Angle
        : operation(base.Angle, abilityModifier.Angle);

    public override float FinalMinRange => abilityModifier.FinalMinRange == 0 || !cntrllBool ? base.FinalMinRange
        : operation(base.FinalMinRange, abilityModifier.FinalMinRange);

    public override float FinalMaxRange => abilityModifier.FinalMaxRange == 0 || !cntrllBool ? base.FinalMaxRange
        : operation(base.FinalMaxRange, abilityModifier.FinalMaxRange);

    public override int FinalMaxDetects => abilityModifier.FinalMaxDetects == 0 || !cntrllBool ? base.FinalMaxDetects
        : (int)operation(base.FinalMaxDetects, abilityModifier.FinalMaxDetects);

    public override float Auxiliar => abilityModifier.Auxiliar == 0 || !cntrllBool ? base.Auxiliar
        : operation(base.Auxiliar, abilityModifier.Auxiliar);

    bool cntrllBool = false;

    //Extension de OperationType?
    float operation(float num, float otherNum)
    {
        if (modificatorBase.operationType == OperationType.add)
            return num + otherNum;
        else
            return num * otherNum;
    }

    public override void ControllerDown(Vector2 dir, float button)
    {

    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        for (int i = 0; i < abilityModifier.damages.Length; i++)
        {
            abilityModifier.damages[i].amount += button;
        }

        //ability.costEnergy > 0  ability.costEnergy+=button : ability.costEnergy-=button
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {

    }
}