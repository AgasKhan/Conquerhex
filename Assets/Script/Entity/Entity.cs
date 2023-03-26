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
}


public class Health
{
    Tim life;
    Tim regen;
    CompleteRoutine timeToRegen;

    public event System.Action damaged;

    public event System.Action noLife;

    public event System.Action death;

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

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
            damaged?.Invoke();

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
        timeToRegen = TimersManager.Create(1, Regen,null);
    }
}
