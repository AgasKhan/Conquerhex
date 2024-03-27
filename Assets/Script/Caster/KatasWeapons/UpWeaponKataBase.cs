using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/UpWeaponKataBase")]
public class UpWeaponKataBase : WeaponKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(UpWeaponKata);
    }
}


/// <summary>
/// Controlador que ejecuta el ataque cuando se suelta el boton de la habilidad
/// </summary>
public class UpWeaponKata : WeaponKata
{
    protected float originalScale;

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack execution", "Ejecuta el ataque cuando se suelta el boton de la habilidad");

        return aux;
    }

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.FeedBackReference = reference;
        aux.SetParent(caster.transform);

        reference.Area(out originalScale);
    }

    //Durante, al mantener y moverlo
    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
        {
            cooldown.Reset();
            return;
        }

        FeedBackReference.Area(originalScale * FinalRange);
        Detect(dir, button);
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        //comienza a bajar el cooldown

        cooldown.Reset();

        Attack();

        FeedBackReference?.Attack();

        End = true;
    }
}

