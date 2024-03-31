using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/UpTriggerControllerBase")]
public class UpTriggerControllerBase : TriggerControllerBase
{
    protected override System.Type SetItemType()
    {
        return typeof(UpTriggerController);
    }
}


/// <summary>
/// Controlador que ejecuta el ataque cuando se suelta el boton de la habilidad
/// </summary>
public class UpTriggerController : TriggerController
{
    protected float originalScale;

    public override void ControllerDown(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

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
            cooldown.Reset();
            return;
        }

        FeedBackReference.Area(originalScale * FinalRange);
        Detect(dir, button);
    }

    //Despues, al sotarlo
    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!onCooldownTime)
            return;

        //comienza a bajar el cooldown

        cooldown.Reset();

        Cast();

        FeedBackReference?.Attack();

        End = true;
    }
}

