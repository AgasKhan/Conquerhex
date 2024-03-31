using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashToEntityUpTriggerControllerBase")]
public class DashToEntityUpTriggerControllerBase : TriggerControllerBase
{
    protected override System.Type SetItemType()
    {
        return typeof(DashToEntityUpTriggerController);
    }
}

public class DashToEntityUpTriggerController : UpTriggerController
{
    Timer timerToEnd;
    bool buttonPress;
    public override void Init(Ability ability)
    {
        base.Init(ability);
        timerToEnd = TimersManager.Create(1, () => End = true).Stop();
    }

    public override void ControllerDown(Vector2 dir, float button)
    {
        buttonPress = true;
        base.ControllerDown(dir, button);
    }

    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();

        if (affected != null && affected.Count != 0 && caster.TryGetComponent<MoveEntityComponent>(out var aux))
        {
            aux.move.Velocity((affected[0].transform.position - caster.transform.position).normalized * ability.itemBase.velocityCharge);
        }

        //Attack();

        //reference?.Attack();

        if (affected.Count == 0)
        {
            End = true;
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        FeedBackReference.Area(originalScale * FinalRange * 1f / 4);
        Detect(Aiming, 0, FinalRange * 1f / 4);

        if (affected.Count == 0)
            return;

        Cast();

        FeedBackReference?.Attack();

        timerToEnd.Stop();

        End = true;
    }
}
