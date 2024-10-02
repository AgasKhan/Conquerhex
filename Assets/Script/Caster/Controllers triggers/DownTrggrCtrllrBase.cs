using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Trigger/DownTriggerControllerBase")]
public class DownTrggrCtrllrBase : UpTrggrCtrllrBase
{
    protected override Type SetItemType()
    {
        return typeof(DownTrggrCtrllre);
    }
}

public class DownTrggrCtrllre : UpTrggrCtrllr
{
    new DownTrggrCtrllrBase triggerBase => base.triggerBase as DownTrggrCtrllrBase;

    public override void ControllerDown(Vector2 dir, float tim)
    {
        ability.FeedbackDetect();

        Detect();

        Cast();

        if (affected != null && affected.Count > 0 && !(triggerBase?.aimingToMove ?? false))
            ObjectiveToAim = (affected[0].transform.position);

        caster.abilityControllerMediator -= this;
    }

    public override void ControllerPressed(Vector2 dir, float tim)
    {
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
    }
}
