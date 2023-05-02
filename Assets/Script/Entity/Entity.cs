using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MyScripts, IDamageable, IGetEntity
{
    public Team team;

    public Health health;

    public List<DropItem> drops = new List<DropItem>();

    SpriteRenderer sprite;

    Color originalColor;

    Timer tim = null;

    IDamageable[] damageables;

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        health.Init();
        health.death += Health_death;

        sprite = GetComponentInChildren<SpriteRenderer>();

        originalColor = sprite.color;

        var aux = GetComponentsInChildren<IDamageable>();

        damageables = new IDamageable[aux.Length - 1];

        //evita el bucle de llamarme a mi mmismo
        int ii = 0;

        for (int i = 0; i < aux.Length; i++)
        {
            if((Object)aux[i]!=this)
            {
                damageables[ii] = aux[i];
                ii++;
            }
        }

   
        tim = TimersManager.Create(0.33f, () => {

            if (((int)(tim.Percentage() * 10)) % 2 == 0)
            {
                //parpadeo rapido


                sprite.color = Color.magenta;
            }
            else
            {
                //el mantenido

                sprite.color = Color.yellow;
            }

        }, () => {

            //volver al original

            sprite.color = originalColor;

        });
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
                //-------------------------------------------------------------
                Instantiate(item.item, transform.position + new Vector3(Random.Range(0, 2.5f), Random.Range(0, 2.5f), 0), Quaternion.identity);
                //-------------------------------------------------------------
            }

            pesoAcumulador += item.peso;
        }

    }

    public virtual void TakeDamage(Damage dmg)
    {
        tim.Reset();

        dmg.ActionInitialiced(this);
        health.TakeLifeDamage(dmg.amount);

        foreach (var item in damageables)
        {
            item.TakeDamage(dmg);
        }
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
    public int peso;

    public GameObject item;
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

    public event System.Action<IGetPercentage> lifeUpdate;

    public event System.Action<IGetPercentage> regenUpdate;

    public event System.Action<float> lifeUpdateAmount;

    public event System.Action<float> regenUpdateAmount;

    public event System.Action noLife;

    public event System.Action reLife;

    public event System.Action death;

    public float TakeRegenDamage(float amount)
    {
        timeToRegen.Reset();

        if (regen.Substract(amount) <= 0 && life.current <= 0)
            death?.Invoke();

        regenUpdate?.Invoke(regen);
        regenUpdateAmount?.Invoke(amount);

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
        
        lifeUpdateAmount?.Invoke(amount);
        

        //actualizar ui
        return life.Percentage();
    }

    void Regen()
    {
        if (life == null && regen == null)
            return;

        bool noLifeBool = false;

        if (life.current <= 0)
            noLifeBool = true;

        life.Substract(-1 * (regen.current / 100f) * life.total);
        regen.Substract(-1);

        if(noLifeBool && life.current>0)
        {
            reLife?.Invoke();
        }

        lifeUpdate?.Invoke(life);
        regenUpdate?.Invoke(regen);
    }

    /// <summary>
    /// Primer parametro vida, segundo regen
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {
        if (timeToRegen != null)
            TimersManager.Destroy(timeToRegen);

        if(param!=null && param.Length>0)
        {
            life = new Tim((float)param[0]);

            regen = new Tim((float)param[1]);
        }

        timeToRegen = TimersManager.Create(3, Regen);
        timeToRegen.SetLoop(true);
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

public enum LifeType
{
    life,
    regen
}

public interface IGetEntity
{
    Entity GetEntity();

    Transform GetTransform();
}