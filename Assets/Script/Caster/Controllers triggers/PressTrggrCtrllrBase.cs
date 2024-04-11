using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PressTriggerControllerBase")]
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
    public override void Set()
    {
        if (pressCooldown != null)
            pressCooldown.Set(GetTrggrBs<PressTrggrCtrllrBase>().timeToAttackPress * FinalVelocity);
        else
            pressCooldown = TimersManager.Create(GetTrggrBs<PressTrggrCtrllrBase>().timeToAttackPress * FinalVelocity);
    }

    public override void ControllerDown(Vector2 dir, float tim)
    {
        if (!onCooldownTime)
            return;

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.FeedBackReference = reference;

        aux.SetParent(caster.transform);

        reference.Area(FinalMaxRange);

        reference.Attack();

        Detect(dir);

        Cast();
        pressCooldown.Reset();
    }

    public override void ControllerPressed(Vector2 dir, float tim)
    {

        if (!onCooldownTime)
        {
            End = true;
            cooldown.Reset();
            return;
        }

        Detect(dir, tim);

        if (pressCooldown.Chck)
        {
            Cast();
            FeedBackReference?.Attack();
            pressCooldown.Reset();
        }
    }

    public override void ControllerUp(Vector2 dir, float tim)
    {
        if (!onCooldownTime)
            return;

        cooldown.Reset();
        pressCooldown.Reset();

        End = true;
    }
}