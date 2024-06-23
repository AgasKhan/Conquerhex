using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityModificators;

[CreateAssetMenu(menuName = "Abilities/OperatorTriggerController")]
public class OperatorTrggrCtrllrBase : ModificatorBase
{
    public TimeController timeController;

    protected override System.Type SetItemType()
    {
        return typeof(OperatorTrggrCtrllr);
    }
}


public class OperatorTrggrCtrllr : Modificator<OperatorTrggrCtrllrBase>
{
    bool cntrllBool = false;

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
        cntrllBool = modificatorBase.timeController == TimeController.Down;
    }

    public override void ControllerPressed(Vector2 dir, float button)
    {
        
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
        cntrllBool = !(modificatorBase.timeController == TimeController.Down);
    }


}