using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MyScripts, IDamageable, IGetEntity
{
    public Team team;

    public Health health;

    public List<DropItem> drops = new List<DropItem>();

    public event System.Action onTakeDamage;

    public event System.Action onDetected;

    public AudioManager audioManager;

    protected abstract Damage[] vulnerabilities { get; }

    IDamageable[] damageables;

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        audioManager = GetComponent<AudioManager>();
        var aux = GetComponentsInChildren<IDamageable>();

        health.Init();

        health.death += Drop;

        health.death += () => { if (gameObject.tag != "Player") gameObject.SetActive(false); };

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
    }

    public void Detect()
    {
        //timDetected.Reset();

        onDetected?.Invoke();
    }

    public void Drop()
    {
        for (int i = 0; i < drops.Count; i++)
        {
            DropItem dropItem = drops[i];

            var rng = dropItem.maxMinDrops.RandomPic();

            for (int ii = 0; ii < rng; ii++)
            {
                PoolManager.SpawnPoolObject(Vector2Int.zero, out RecolectableItem reference, transform.position + new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-1.2f, 1.2f)));

                reference.Init(dropItem.item);
            }
        }
    }

    public virtual void TakeDamage(Damage dmg)
    {
        onTakeDamage?.Invoke();

        if(vulnerabilities!=null)
            for (int i = 0; i < vulnerabilities.Length; i++)
            {
                if (dmg.typeInstance == vulnerabilities[i].typeInstance)
                {
                    dmg.amount *= vulnerabilities[i].amount;
                    break;
                }
            }

        dmg.ActionInitialiced(this);
        health.TakeLifeDamage(dmg.amount);

        foreach (var item in damageables)
        {
            item.TakeDamage(dmg);
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
        tim = TimersManager.Create(time, update,
            () =>
            {
                end?.Invoke();
                health.death -= internalEnd;
            });
    }

    public Entity GetEntity()
    {
        return this;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

[System.Serializable]
public struct DropItem
{
    public Pictionarys<int, int> maxMinDrops;

    public ResourcesBase_ItemBase item;
}

[System.Serializable]
public class Health : Init
{
    [SerializeField]
    public Tim life;
    [SerializeField]
    public Tim regen;
    [SerializeField]
    TimedAction timeToRegen;

    public float maxLife => life.total;
    public float actualLife => life.current;
    public float maxRegen => regen.total;
    public float actualRegen => regen.current;

    /// <summary>
    /// porcentaje, actual e input
    /// </summary>
    public event System.Action<IGetPercentage, float, float> lifeUpdate;

    /// <summary>
    /// porcentaje, actual e input
    /// </summary>
    public event System.Action<IGetPercentage, float, float> regenUpdate;

    public event System.Action noLife;

    public event System.Action reLife;

    public event System.Action death;

    bool deathBool = false;

    public void StartRegenTimer()
    {
        timeToRegen.Start();
    }

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

        if (regen.Substract(amount) <= 0 && life.current <= 0 && !deathBool)
        {
            death?.Invoke();
            deathBool = true;
        }

        regenUpdate?.Invoke(regen, actualRegen, amount);

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

        lifeUpdate?.Invoke(life, actualLife, amount);

        //actualizar ui
        return life.Percentage();
    }

    void Regen()
    {
        if (life == null && regen == null && deathBool)
            return;

        bool noLifeBool = false;

        if (life.current <= 0)
            noLifeBool = true;

        /*
        life.Substract(-1 * (regen.current / 100f) * life.total);
        regen.Substract(-1);
        */

        TakeLifeDamage(-1 * (regen.current / 100f) * life.total);
        TakeRegenDamage(-1);

        if (noLifeBool && life.current > 0)
        {
            reLife?.Invoke();
        }


        if (regen.current == regen.total && life.current == life.total)
            timeToRegen.Stop();
    }

    /// <summary>
    /// Primer parametro vida, segundo regen
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {
        if (timeToRegen != null)
            TimersManager.Destroy(timeToRegen);

        if (param != null && param.Length > 0)
        {
            life = new Tim((float)param[0]);

            regen = new Tim((float)param[1]);
        }

        timeToRegen = TimersManager.Create(3, Regen);
        timeToRegen.SetLoop(true).Stop();
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
    public const string life = "life", regen = "regen";
}

public interface IGetEntity
{
    Entity GetEntity();
}
