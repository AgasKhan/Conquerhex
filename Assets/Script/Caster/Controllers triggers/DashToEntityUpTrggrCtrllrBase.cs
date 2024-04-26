using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashToEntityUpTriggerControllerBase")]
public class DashToEntityUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash=10;

    public float timerDash = 1;

    public int dashCount = 1;
    protected override System.Type SetItemType()
    {
        return typeof(DashToEntityUpTrggrCtrllr);
    }
}

[System.Serializable]
public class DashToEntityUpTrggrCtrllr : UpTrggrCtrllr
{
    MoveEntityComponent moveEntity;
    Timer timerToEnd;
    bool buttonPress;
    int dashCount;

    new public DashToEntityUpTrggrCtrllrBase triggerBase => (DashToEntityUpTrggrCtrllrBase)base.triggerBase;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        timerToEnd = TimersManager.Create(triggerBase.timerDash, () => End = true).Stop();
    }

    public override void ControllerDown(Vector2 dir, float button)
    {
        base.ControllerDown(dir, button);
        if (End)
        {
            return;
        }

        buttonPress = true;
        
        FeedBackReference?.DotAngle(Dot);
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

        if (affected != null && affected.Count != 0 && caster.TryGetComponent<MoveEntityComponent>(out moveEntity))
        {
            moveEntity.Velocity((affected[0].transform.position - caster.transform.position).normalized , triggerBase.velocityInDash);
        }
        else
        {
            Cast();
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
        dashCount = triggerBase.dashCount;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        FeedBackReference?.Area(originalScale * FinalMaxRange * 1f / 4, originalScale * FinalMinRange * 1f / 4);

        Detect(0, FinalMaxRange * 1f / 4);

        if (affected.Count == 0)
            return;
       
        FeedBackReference?.Attack();

        Cast();

        Detect(0, FinalMaxRange*4);

        dashCount--;

        if (dashCount <= 0 || affected.Count < 2)
        {
            timerToEnd.Stop();
            moveEntity.VelocityCalculate = Vector3.zero;
            return;
        }

        timerToEnd.Reset();

        End = false;

        Aiming = (affected[1].transform.position - caster.transform.position).normalized;

        moveEntity.Velocity(Aiming, triggerBase.velocityInDash);

        FeedBackReference?.Area(originalScale * FinalMaxRange, originalScale * FinalMinRange).Direction(Aiming);
    }
}
