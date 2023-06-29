using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador que ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)
/// </summary>
public class PressWeaponKata : WeaponKata
{
    public Timer pressCooldown;

    public override void Init(params object[] param)
    {
        base.Init(param);
        if (weapon != null)
            pressCooldown = TimersManager.Create(finalVelocity*1.5f);
    }

    public override void ChangeWeapon(int weaponIndex)
    {
        base.ChangeWeapon(weaponIndex);
        pressCooldown.Set(finalVelocity * 1.5f);
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

        this.reference = reference;

        aux.SetParent(caster.transform);

        reference.Area(finalRange);

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
            reference.Attack();
            pressCooldown.Reset();
        }
    }

    protected override void InternalControllerUp(Vector2 dir, float tim)
    {
        reference.Off();
        cooldown.Reset();
        pressCooldown.Reset();
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

        this.reference = reference;
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

        reference.Area(originalScale* finalRange);
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

        reference.Off().Attack();
    }
}

public class DashUpWeaponKata : UpWeaponKata
{
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();

        if (affected != null && affected.Length!=0 && caster is DinamicEntity)
        {
            var aux = caster as DinamicEntity;

            aux.move.Velocity((affected[0].transform.position - caster.transform.position).normalized * itemBase.velocityCharge);
        }

        Attack();

        reference.Off().Attack();
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeAffectedUpWeaponKata : UpWeaponKata
{
    protected override Entity[] InternalDetect(Vector2 dir, float timePressed = 0)
    {
        return itemBase.Detect(caster, dir ,(int)Mathf.Clamp(timePressed * itemBase.velocityCharge, 1, itemBase.detect.maxDetects), finalRange);
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeRangeUpWeaponKata : UpWeaponKata
{
    public override float finalRange => Mathf.Clamp(range * itemBase.velocityCharge, 1, base.finalRange);

    float range;

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        range = 0;
        base.InternalControllerDown(dir, button);
    }

    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        range = button;
        base.InternalControllerPress(dir, button);
    }
}