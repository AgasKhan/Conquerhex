using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/PressWeaponKataBase")]
public class PressWeaponKataBase : WeaponKataBase
{
    [Tooltip("Multiplicador de espera para el golpe automatico")]
    public float timeToAttackPress;

    public override Item Create()
    {
        PressWeaponKata aux = base.Create() as PressWeaponKata;
        aux.pressCooldown = TimersManager.Create(timeToAttackPress*velocity);

        return aux;
    }

    protected override System.Type SetItemType()
    {
        return typeof(PressWeaponKata);
    }
}

/// <summary>
/// Controlador que ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)
/// </summary>
public class PressWeaponKata : WeaponKata
{
    public Timer pressCooldown;

    public override void ChangeWeapon(Item meleeWeapon)
    {
        base.ChangeWeapon(meleeWeapon);

        if (pressCooldown != null)
            pressCooldown.Set(FinalVelocity * 1.5f);
        else
            pressCooldown = TimersManager.Create(FinalVelocity * 1.5f);
    }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack execution", "Ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)");

        return aux;
    }

    protected override void InternalControllerDown(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
            return;

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.FeedBackReference = reference;

        aux.SetParent(caster.transform);

        reference.Area(FinalRange);

        reference.Attack();

        Detect(dir);

        Attack();
        pressCooldown.Reset();
    }

    protected override void InternalControllerPress(Vector2 dir, float tim)
    {

        if (!cooldown.Chck)
        {
            cooldown.Reset();
            return;
        }

        Detect(dir, tim);

        if (pressCooldown.Chck)
        {
            Attack();
            FeedBackReference?.Attack();
            pressCooldown.Reset();
        }
    }

    protected override void InternalControllerUp(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();
        pressCooldown.Reset();

        End = true;
    }
}