using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Trigger/PressTriggerControllerBase")]
public class PressTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Multiplicador de espera para el golpe automatico")]
    public float timeToAttackPress;

    protected override System.Type SetItemType()
    {
        return typeof(PressTrggrCtrllr);
    }
}

/// <summary>
/// Controlador que ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)
/// </summary>
public class PressTrggrCtrllr : TriggerController
{
    public Timer pressCooldown;

    new public PressTrggrCtrllrBase triggerBase => (PressTrggrCtrllrBase)base.triggerBase;

    bool inCast;
    public override void Set()
    {
        if (pressCooldown != null)
            pressCooldown.Set(triggerBase.timeToAttackPress * FinalVelocity);
        else
            pressCooldown = TimersManager.Create(triggerBase.timeToAttackPress * FinalVelocity);
    }

    public override void ControllerDown(Vector2 dir, float tim)
    {
        ability.FeedbackDetect();

        Detect();

        inCast = true;
        
        Cast(() =>
        {
            End = false;
            inCast = false;
        });

        pressCooldown.Reset();
    }

    public override void ControllerPressed(Vector2 dir, float tim)
    {
        ability.FeedbackDetect();

        Detect();
        
        if (pressCooldown.Chck)
        {
            inCast = true;

            Cast(() =>
            {
                End = false;
                inCast = false;
            });

            pressCooldown.Reset();
        }
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
        if(inCast)
        {
            ability.onEndAction += (a) => End = true;
        }
        else
            End = true;

        pressCooldown.Stop();
    }
}