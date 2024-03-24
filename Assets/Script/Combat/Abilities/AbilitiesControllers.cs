using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador que ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)
/// </summary>
public class PressWeaponKata : WeaponKata
{
    public Timer pressCooldown;

    public override void ChangeWeapon(Item meleeWeapon)
    {
        base.ChangeWeapon(meleeWeapon);

        if(pressCooldown!=null)
            pressCooldown.Set(finalVelocity * 1.5f);
        else
            pressCooldown = TimersManager.Create(finalVelocity * 1.5f);
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
            reference?.Attack();
            pressCooldown.Reset();
        }
    }

    protected override void InternalControllerUp(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();
        pressCooldown.Reset();

        end = true;
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

        reference?.Attack();

        end = true;
    }
}

public class DashUpWeaponKata : UpWeaponKata
{
    Timer timerToEnd;
    bool buttonPress;
    protected override void Init()
    {
        base.Init();
        timerToEnd = TimersManager.Create(1, ()=> end=true).Stop();
    }

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        buttonPress = true;
        base.InternalControllerDown(dir, button);
    }

    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();

        if (affected != null && affected.Length!=0 && caster.TryGetComponent<MoveEntityComponent>(out var aux))
        {
            aux.move.Velocity((affected[0].transform.position - caster.transform.position).normalized * itemBase.velocityCharge);
        }

        //Attack();

        //reference?.Attack();

        if(affected.Length==0)
        {
            end = true;
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        reference.Area(originalScale * finalRange * 1f / 4);
        Detect(Aiming, 0, finalRange* 1f/4);

        if (affected.Length == 0)
            return;

        Attack();

        reference?.Attack();

        timerToEnd.Stop();

        end = true;
    }
}

/// <summary>
/// 
/// </summary>
public class ChargeAffectedUpWeaponKata : UpWeaponKata
{
    protected override Entity[] InternalDetect(Vector2 dir, float timePressed = 0, float? range=null)
    {
        return itemBase.Detect(caster.container, dir ,(int)Mathf.Clamp(timePressed * itemBase.velocityCharge, 1, itemBase.detect.maxDetects), finalRange);
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