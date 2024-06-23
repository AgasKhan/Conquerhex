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
    bool cntrllBool = false;

    //Extension de OperationType?

    protected override float Operation(float previusValue, float flyweightValue)
    {
        if (!cntrllBool)
            return previusValue;

        if (modificatorBase.operationType == OperationType.add)
            return previusValue + flyweightValue;
        else
            return previusValue * flyweightValue;
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