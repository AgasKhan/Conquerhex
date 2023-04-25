using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MyScripts, IDamageable
{
    public Team team;

    public Health health;

    public List<DropItem> drops = new List<DropItem>();

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        health.Init();
        health.death += Health_death;
    }
    

    public void Drop()
    {
        int aux=0;

        foreach (var item in drops)
        {
            aux += item.peso;
        }

        aux = Random.Range(0, aux);

        int pesoAcumulador = 0;

        foreach (var item in drops)
        {
            if((pesoAcumulador + item.peso)> aux)
            {
                //return item.item;
            }

            pesoAcumulador += item.peso;
        }

    }

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

    private void Health_death()
    {
        foreach (var item in drops)
        {
            Debug.Log(item);
        }
    }
}

public struct DropItem
{
    public int peso;

    public ItemBase item;
}

[System.Serializable]
public class Health : Init
{
    [SerializeField]
    Tim life;
    [SerializeField]
    Tim regen;
    [SerializeReference]
    Routine timeToRegen;

    public event System.Action<IGetPercentage> lifeUpdate;

    public event System.Action<IGetPercentage> regenUpdate;

    public event System.Action noLife;

    public event System.Action reLife;

    public event System.Action death;

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

        if (regen.Substract(amount) <= 0 && life.current <= 0)
            death?.Invoke();

        regenUpdate?.Invoke(regen);
        return regen.Percentage();
    }

    public float TakeLifeDamage(float amount)
    {
        timeToRegen.Reset();

        if (life.current - amount <= 0)
        {
            var aux = amount - life.current;
            
            noLife?.Invoke();

            life.Substract(amount);

            TakeRegenDamage(aux);
        }
        else
        {
            life.Substract(amount);                
        }

        lifeUpdate?.Invoke(life);

        //actualizar ui
        return life.Percentage();
    }

    void Regen()
    {
        bool noLifeBool = false;

        if (life.current <= 0)
            noLifeBool = true;

        life.Substract(-1 * (regen.current / 100f) * life.total);
        regen.Substract(-1);
        timeToRegen.Reset();

        if(noLifeBool && life.current>0)
        {
            reLife?.Invoke();
        }

        lifeUpdate?.Invoke(life);
        regenUpdate?.Invoke(regen);
    }

    public void Init(params object[] param)
    {
        timeToRegen = TimersManager.Create(3, Regen, false);
    }
}

public enum Team
{
    windows,
    mac
}