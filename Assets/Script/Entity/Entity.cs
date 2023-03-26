using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    public Health health;
    public virtual void TakeDamage(Damage dmg)
    {
        dmg.ActionInitialiced(this);
        health.TakeLifeDamage(dmg.amount);
    }

    /// <summary>
    /// Crea un efecto de estado para el personaje
    /// </summary>
    /// <param name="time">tiempo que deseo que dure el efecto</param>
    /// <param name="update">que deseo realizar mientras este activo el efecto</param>
    /// <param name="end">que deseo realizar cuando termine el efecto</param>
    public void Effect(float time, System.Action update, System.Action end)
    {
        Tim tim = null;

        //se ejecutara cuando muere el personaje
        System.Action internalEnd = 
        () =>
        {
            //lleva el timer a 0, haciendo que la funcion de fin del timer
            tim.Set(0);
        };

        //agrego al evento de muerte la funcion que deseo ejecutar en ese caso
        health.death += internalEnd;

        //creo el timer, que se encargara de manejar el flujo
        tim = TimersManager.Create(time,update,
            () =>
            {
                end?.Invoke();
                health.death -= internalEnd;
            });
    }
}


public class Health
{
    Tim life;
    Tim regen;
    Routine timeToRegen;

    public event System.Action regenDamaged;

    public event System.Action lifeDamaged;

    public event System.Action noLife;

    public event System.Action death;

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

        regenDamaged?.Invoke();

        var aux = regen.Substract(amount);

        if (aux <= 0 && life.current <= 0)
            death?.Invoke();

        //actualizar ui
        return aux;
    }

    public float TakeLifeDamage(float amount)
    {
        timeToRegen.Reset();

        if (life.current - amount <= 0)
        {
            var aux = amount - life.current;
            
            noLife?.Invoke();

            if (TakeRegenDamage(aux) <= 0)
            {
                death?.Invoke();
                return 0;
            }
        }

        if(amount>0)
            lifeDamaged?.Invoke();

        //actualizar ui
        return life.Substract(amount);
    }

    void Regen()
    {
        life.Substract(-1 * (regen.current / 100) * life.total);
        regen.Substract(-1);
    }

    public Health()
    {
        timeToRegen = TimersManager.Create(1, Regen);
    }
}
