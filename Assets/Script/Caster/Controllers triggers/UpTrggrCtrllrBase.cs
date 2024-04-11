using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/UpTriggerControllerBase")]
public class UpTrggrCtrllrBase : TriggerControllerBase
{
    protected override System.Type SetItemType()
    {
        return typeof(UpTrggrCtrllr);
    }
}


/// <summary>
/// Controlador que ejecuta el ataque cuando se suelta el boton de la habilidad
/// </summary>
public class UpTrggrCtrllr : TriggerController
{
    protected float originalScale;

    public override void ControllerDown(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            cooldown.Reset();
            return;
        }

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.FeedBackReference = reference;
        aux.SetParent(caster.transform);

        reference.Area(out originalScale);
    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            cooldown.Reset();
            return;
        }

        Aiming = Vector2.Lerp(Aiming, dir, Time.deltaTime);

        FeedBackReference.Area(originalScale * FinalMaxRange).Direction(Aiming);

        Detect(Aiming, button);
    }

    //Despues, al sotarlo
    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            cooldown.Reset();
            return;
        }

        //comienza a bajar el cooldown

        cooldown.Reset();

        Cast();

        FeedBackReference?.Attack();

        End = true;
    }
}

