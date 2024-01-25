using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MyScripts, IDamageable, IGetEntity
{
    public Team team;
    
    public Health health;

    public List<DropItem> drops = new List<DropItem>();

    public event System.Action<Damage> onTakeDamage;

    public event System.Action onDetected;

    public AudioManager audioManager;

    public CarlitoEntity carlitosPrefab;

    [field : SerializeField]
    public Transform[] carlitos { get; private set; }

    protected abstract Damage[] vulnerabilities { get; }
    
    public virtual bool visible { get => isActiveAndEnabled; set => enabled = value; }

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

        //health.death += () => { if (gameObject.tag != "Player") gameObject.SetActive(false); };

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

        if (carlitosPrefab == null)
            return;

        carlitos = new Transform[6];

        for (int i = 0; i < carlitos.Length; i++)
        {
            carlitos[i] = Instantiate(carlitosPrefab, transform).transform;

            carlitos[i].name = "Carlitos (" + i + ")";

            carlitos[i].transform.SetPositionAndRotation(transform.position, transform.rotation);

            //carlitos[i].SetActiveGameObject(false);
        }

        LoadSystem.AddPostLoadCorutine(()=> {
            Hexagone hexagone = GetComponentInParent<Hexagone>();

            if (hexagone != null)
            {
                Teleport(hexagone, 0);
            }
        });
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

    public void TakeDamage(params Damage[] dmgs)
    {
        if (dmgs == null)
            return;

        //string notif = "";

        for (int i = 0; i < dmgs.Length; i++)
        {
            TakeDamage(ref dmgs[i]);

            //notif += dmgs[i] + " ";

            Interfaz.instance?.PopTextDamage(this ,dmgs[i].ToString());
        }      

        //Interfaz.instance?["Danio"].AddMsg($"{notif} ► {name.Replace("(Clone)","")}");
    }

    public virtual void TakeDamage(ref Damage dmg)
    {
        if(vulnerabilities!=null)
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

        onTakeDamage?.Invoke(dmg);

        dmg.ActionInitialiced(this);
        health.TakeLifeDamage(dmg.amount);

        foreach (var item in damageables)
        {
            item.TakeDamage(ref dmg);
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
        Timer tim = null;

        //se ejecutara cuando muere el personaje
        System.Action internalEnd =
        () =>
        {
            //lleva el timer a 0, haciendo que la funcion de fin del timer
            tim.SetInitCurrent(0);
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
    Tim life;
    [SerializeField]
    Tim regen;
    [SerializeField]
    TimedAction timeToRegen;

    public float maxLife => life.total;
    public float actualLife => life.current;
    public float maxRegen => regen.total;
    public float actualRegen => regen.current;
    public float actualCoolDownRegen => timeToRegen.current;
    public float MaxCoolDownRegen => timeToRegen.total;

    public float nextRegenLife => (regen.current / 100f) * life.total + life.current;

    public event System.Action<Health> helthUpdate;

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

    public event System.Action noLife;

    public event System.Action reLife;

    public event System.Action death;

    [SerializeField]
    bool deathBool = false;

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

        if (regen.Substract(amount) <= 0 && life.current <= 0 && !deathBool)
        {
            death?.Invoke();
            deathBool = true;
        }

        //regenUpdate?.Invoke(regen, amount);

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

        //lifeUpdate?.Invoke(life, amount);

        //actualizar ui
        return life.Percentage();
    }

    void Regen()
    {
        if (life == null || regen == null)
            return;

        if (regen.current == regen.total && life.current == life.total || deathBool)
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
    public void Init(params object[] param)
    {
        if (timeToRegen != null)
            timeToRegen.Stop();

        if (param != null && param.Length > 0)
        {
            life = new Tim((float)param[0]);

            regen = new Tim((float)param[1]);

            life.onChange += (a,b) => helthUpdate?.Invoke(this);

            regen.onChange += (a, b) => helthUpdate?.Invoke(this);
        }

        timeToRegen = TimersManager.Create(3, Regen);

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
