﻿using ComponentsAndContainers;
using System.Collections.Generic;
using UnityEngine;
public abstract class Entity : Container<Entity>, IDamageable, IGetEntity
{

    public const float tickTimeDamage = 1 / 3f;

    public struct StateDamage
    {
        public Timer endCheck;

        public System.Action tickDamageEnd;

        public System.Action updateTick;
    }
    public Team team;
    
    public Health health;

    public event System.Action<Damage> onTakeDamage;

    public event System.Action onDetected;

    [field : SerializeField]
    public Transform[] carlitos { get; private set; }

    [field: SerializeField]
    public EntityBase flyweight {get; protected set;}

    [field: SerializeField]
    public HealthBase healthBase { get; protected set; }

    protected Damage[] vulnerabilities => healthBase?.vulnerabilities;
    
    public virtual bool visible { get => isActiveAndEnabled; set => enabled = value; }

    IDamageable[] damageables;

    TimedAction tickDamage;

    System.Action updateTickDamage;

    public void Detect()
    {
        //timDetected.Reset();

        onDetected?.Invoke();
    }

    public void TakeDamage(Damage dmg)
    {
        InternalTakeDamage(ref dmg);

        UI.Interfaz.instance?.PopTextDamage(this, dmg.ToString());

        //Interfaz.instance?["Danio"].AddMsg($"{notif} ► {name.Replace("(Clone)","")}");
    }

    public void TakeDamage(IEnumerable<Damage> dmgs)
    {
        if (dmgs == null)
            return;

        //string notif = "";

        foreach (var dmgFor in dmgs)
        {
            TakeDamage(dmgFor);
        }

        //Interfaz.instance?["Danio"].AddMsg($"{notif} ► {name.Replace("(Clone)","")}");
    }

    public virtual void InternalTakeDamage(ref Damage dmg)
    {
        if (vulnerabilities!=null)
            for (int i = 0; i < vulnerabilities.Length; i++)
            {
                if (dmg.typeInstance == vulnerabilities[i].typeInstance)
                {
                    dmg.amount *= vulnerabilities[i].amount;
                    break;
                }
            }

        if (dmg.amount <= 0)
            return;

        switch (dmg.typeInstance.target)
        {
            case DamageTypes.Target.all:

                if (health.actualLife > 0 || health.actualRegen > 0)
                {
                    health.TakeAllDamage(dmg.amount);
                    onTakeDamage?.Invoke(dmg);
                }
                else
                    dmg.amount = 0;

            break;

            case DamageTypes.Target.life:

                if (health.actualLife > 0)
                {
                    health.TakeLifeDamage(dmg.amount);
                    onTakeDamage?.Invoke(dmg);
                }
                else
                    dmg.amount = 0;

            break;

            case DamageTypes.Target.regen:

                if (health.actualRegen > 0)
                {
                    health.TakeRegenDamage(dmg.amount);
                    onTakeDamage?.Invoke(dmg);
                }
                else
                    dmg.amount = 0;

            break;

            case DamageTypes.Target.maxLife:
                if (health.actualLife > 0)
                {
                    health.TakeMaxLifeDamage(dmg.amount);
                    onTakeDamage?.Invoke(dmg);
                }
                else
                    dmg.amount = 0;

            break;

            case DamageTypes.Target.maxRegen:

                if (health.actualRegen > 0)
                {
                    health.TakeMaxRegenDamage(dmg.amount);
                    onTakeDamage?.Invoke(dmg);
                }
                else
                    dmg.amount = 0;
            break;
                /*
            default:

                break;
                */
        }

        dmg.ActionInitialiced(this, dmg.amount);

        foreach (var item in damageables)
        {
            item.InternalTakeDamage(ref dmg);
        }

        health.StartRegenTimer();
    }

    /// <summary>
    /// Crea un efecto de estado para el personaje
    /// </summary>
    /// <param name="time">tiempo que deseo que dure el efecto</param>
    /// <param name="update">que deseo realizar mientras este activo el efecto</param>
    /// <param name="end">que deseo realizar cuando termine el efecto</param>
    public void Effect(float time, System.Action update, System.Action end)
    {
        StateDamage stateDamage;

        stateDamage.endCheck = TimersManager.Create(time);

        stateDamage.updateTick = null;

        //se ejecutara cuando muere el personaje
        stateDamage.tickDamageEnd =
        () =>
        {
            //lleva el timer a 0, haciendo que la funcion de fin del timer
            stateDamage.endCheck.SetInitCurrent(0);
        };

        stateDamage.updateTick = () =>
        {
            update?.Invoke();

            if (stateDamage.endCheck.Chck)
            {
                end?.Invoke();
                health.death -= stateDamage.tickDamageEnd;
                updateTickDamage -= stateDamage.updateTick;

                if(updateTickDamage==null)
                    tickDamage.Stop();
            }
        };

        //agrego al evento de muerte la funcion que deseo ejecutar en ese caso
        health.death += stateDamage.tickDamageEnd;

        //lo agrego el update tick
        updateTickDamage += stateDamage.updateTick;

        //si esta frenado lo reinicio
        if(tickDamage.Freeze)
            tickDamage.Reset();
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public Entity GetEntity()
    {
        if (visible)
            return this;
        else
            return null;
    }

    public void Teleport(Hexagone hexagone, int lado)
    {
        hexagone.SetProyections(transform,carlitos);

        for (int i = 0; i < carlitos.Length; i++)
        {
            if (hexagone.ladosArray[i].id == hexagone.id)
                carlitos[i].SetActiveGameObject(false);
            else
                carlitos[i].SetActiveGameObject(true);
        }
    }

    protected override void Config()
    {
        base.Config();
        MyAwakes = MyAwakes + MyAwake;
    }

    private void MyAwake()
    {
        var aux = GetComponentsInChildren<IDamageable>();

        if (healthBase == null)
            healthBase = flyweight.GetFlyWeight<HealthBase>();

        tickDamage = TimersManager.Create(tickTimeDamage, ()=> updateTickDamage?.Invoke() ).SetLoop(true).Stop() as TimedAction;

        health.Init(healthBase.life, healthBase.regen);

        damageables = new IDamageable[aux.Length - 1];

        int ii = 0;

        for (int i = 0; i < aux.Length; i++)
        {
            if ((Object)aux[i] != this)
            {
                damageables[ii] = aux[i];
                ii++;
            }
        }

        carlitos = new Transform[6];

        for (int i = 0; i < carlitos.Length; i++)
        {
            carlitos[i] = Instantiate(BaseData.Carlitos, transform).transform;

            carlitos[i].name = "Carlitos (" + i + ") de " + name;

            carlitos[i].transform.SetPositionAndRotation(transform.position, transform.rotation);

            //carlitos[i].SetActiveGameObject(false);
        }

        LoadSystem.AddPostLoadCorutine(() => {
            Hexagone hexagone = GetComponentInParent<Hexagone>();

            if (hexagone != null)
            {
                Teleport(hexagone, 0);
            }
        });
    }
}



[System.Serializable]
public class Health
{
    public event System.Action noLife;
    public event System.Action reLife;
    public event System.Action death;

    public event System.Action<Health> helthUpdate;

    [SerializeField]
    Tim life;
    [SerializeField]
    Tim regen;
    [SerializeField]
    TimedAction timeToRegen;

    const int multiplyTimerRegenMax = 2;

    const float tickTimerRegen = 3;
    
    [SerializeField]
    bool deathBool = false;

    [SerializeField]
    bool regenCancelStop = false;

    public float maxLife => life.total;
    public float actualLife => life.current;
    public float maxRegen => regen.total;
    public float actualRegen => regen.current;
    public float actualCoolDownRegen => timeToRegen.current;
    public float MaxCoolDownRegen => timeToRegen.total;
    public float nextRegenLife => (regen.current / 100f) * life.total + life.current;
    
    public event System.Action<IGetPercentage, float> lifeUpdate
    {
        add
        {
            life.onChange += value;
        }
        remove
        {
            life.onChange -= value;
        }
    }

    public event System.Action<IGetPercentage, float> regenUpdate
    {
        add
        {
            regen.onChange += value;
        }
        remove
        {
            regen.onChange -= value;
        }
    }

    public event System.Action<IGetPercentage, float> regenTimeUpdate
    {
        add
        {
            timeToRegen.onChange += value;
        }
        remove
        {
            timeToRegen.onChange -= value;
        }
    }

    public void StartRegenTimer()
    {
        timeToRegen.Start();
    }

    public void Revive()
    {
        regen.Reset();
        life.Reset();
        deathBool = false;
    }

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

        regen.Substract(amount);

        DeathCheck();

        return regen.Percentage();
    }

    public float TakeMaxRegenDamage(float amount)
    {
        timeToRegen.Reset();

        regen.total-= amount;

        int cantidad = multiplyTimerRegenMax;

        regenCancelStop = true;

        System.Action action = null;

        action = () =>
        {
            cantidad--;

            if (cantidad <= 0)
            {
                timeToRegen.SubstractToEnd(action);
                regen.total += amount;

                regenCancelStop = false;
            }
        };

        timeToRegen.AddToEnd(action);

        DeathCheck();

        return regen.Percentage();
    }

    public float TakeLifeDamage(float amount)
    {
        timeToRegen.Reset();

        life.Substract(amount);

        DeathCheck();

        return life.Percentage();
    }

    public float TakeMaxLifeDamage(float amount)
    {
        timeToRegen.Reset();

        life.total -= amount;

        int cantidad = multiplyTimerRegenMax;

        regenCancelStop = true;

        System.Action action = null;

        action = () =>
        {
            cantidad--;

            if(cantidad<=0)
            {
                timeToRegen.SubstractToEnd(action);
                life.total += amount;
                regenCancelStop = false;
            }
        };

        timeToRegen.AddToEnd(action);

        DeathCheck();

        return life.Percentage();
    }

    public float TakeAllDamage(float amount)
    {
        if (life.current - amount <= 0)
        {
            var aux = amount - life.current;

            TakeLifeDamage(amount);

            TakeRegenDamage(aux);
        }
        else
        {
            TakeLifeDamage(amount);
        }

        return life.Percentage();
    }

    void DeathCheck()
    {
        if(!deathBool)
        {
            if (regen.current <= 0 && life.current <= 0)
            {
                death?.Invoke();
                deathBool = true;
            }
            else if (life.current <= 0)
            {
                noLife?.Invoke();
            }
        }
    }

    void Regen()
    {
        if (life == null || regen == null)
            return;

        if ((!regenCancelStop && regen.current == regen.total && life.current == life.total) || deathBool)
        {
            timeToRegen.Stop();
            return;
        }
            
        bool noLifeBool = life.current <= 0;

        TakeLifeDamage(-1 * (regen.current / 100f) * life.total);
        TakeRegenDamage(-1);

        if (noLifeBool && life.current > 0)
        {
            reLife?.Invoke();
        }
    }

    /// <summary>
    /// Primer parametro vida, segundo regen
    /// </summary>
    /// <param name="param"></param>
    public void Init(float lifeNumber = -1, float regenNumber =-1)
    {
        if (timeToRegen != null)
            timeToRegen.Stop();

        if (lifeNumber>0)
        {
            life = new Tim(lifeNumber);

            life.onChange += (a,b) => helthUpdate?.Invoke(this);
        }

        if (regenNumber > 0)
        {
            regen = new Tim(regenNumber);

            regen.onChange += (a, b) => helthUpdate?.Invoke(this);
        }

        timeToRegen = TimersManager.Create(tickTimerRegen, Regen);

        timeToRegen.SetLoop(true);

        timeToRegen.onChange += (a, b) => helthUpdate?.Invoke(this);
    }
}

public enum Team
{
    player,
    enemy,
    hervivoro,
    recursos,
    carnivoro
}

public static class LifeType
{
    public const string life = "life", regen = "regen", time = "time", all = "all";
}

public interface IGetEntity
{
    Entity GetEntity();

    bool visible { get; set; }

    Transform transform { get;}
}
