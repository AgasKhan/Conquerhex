using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashToEntityUpTriggerControllerBase")]
public class DashToEntityUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash=10;

    public float timerDash = 1;
    protected override System.Type SetItemType()
    {
        return typeof(DashToEntityUpTrggrCtrllr);
    }
}

public class DashToEntityUpTrggrCtrllr : UpTrggrCtrllr
{
    Timer timerToEnd;
    bool buttonPress;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        timerToEnd = TimersManager.Create(GetTrggrBs<DashToEntityUpTrggrCtrllrBase>().timerDash, () => End = true).Stop();
    }

    public override void ControllerDown(Vector2 dir, float button)
    {
        base.ControllerDown(dir, button);
        if (End)
        {
            return;
        }

        buttonPress = true;
        
        FeedBackReference.DotAngle(Dot);
    }

    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
        {
            End = true;
            cooldown.Reset();
            return;
        }
        
        cooldown.Reset();

        if (affected != null && affected.Count != 0 && caster.TryGetComponent<MoveEntityComponent>(out var aux))
        {
            aux.move.Velocity((affected[0].transform.position - caster.transform.position).normalized * GetTrggrBs<DashToEntityUpTrggrCtrllrBase>().velocityInDash);
        }
        else
        {
            Cast();
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

        FeedBackReference.Area(originalScale * FinalMaxRange * 1f / 4);
        Detect(Aiming, 0, FinalMaxRange * 1f / 4);

        if (affected.Count == 0)
            return;

        Cast();

        if (caster.TryGetComponent<MoveEntityComponent>(out var aux))
        {
            aux.move.velocity = 0;
        }

        FeedBackReference?.Attack();

        timerToEnd.Stop();

        End = true;
    }
}
