using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(Damage dmg);
}

[System.Serializable]
public struct Damage
{
    public float amount;
    public EnumDamage type;
    public ClassDamage typeInstance;

    public void Init()
    {
        ClassDamage.SearchDamage(type);
        Debug.Log("me ejecute");
    }

    public void ActionInitialiced(Entity go)
    {
        typeInstance.IntarnalAction(go, amount);
    }

}

public enum EnumDamage
{
    Slash,
    Impact,
    Perforation,
    Toxic
}

public abstract class ClassDamage
{
    static Dictionary<System.Type, ClassDamage> typesDamages = new Dictionary<System.Type, ClassDamage>();

    public ClassDamage()
    {
        typesDamages.Add(this.GetType(), this);
    }

    public static ClassDamage SearchDamage(EnumDamage type)
    {
        foreach (var item in typesDamages)
        {
            if (item.GetType().Name == type.ToString())
            {
                return item.Value;
            }
        }

        return null;
    }

    public abstract void IntarnalAction(Entity go, float amount);
}


/// <summary>
/// este es un daño elemental
/// </summary>
public abstract class Elemental : ClassDamage
{
}

/// <summary>
/// este es el daño fisico
/// </summary>
public abstract class Physic : ClassDamage
{
}

public class Slash : Physic
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        Tim tim=null;

        System.Action end = () =>
        {
            tim.Set(0);
        };

        entity.health.death += end;

        tim = TimersManager.Create(amount/3,
            ()=> 
            {
                entity.health.TakeRegenDamage(Time.deltaTime);
            },
            ()=>
            {
                entity.health.death -= end;
            });
    }
}

/// <summary>
/// danio extra aleatorio de hasta el 50%
/// </summary>
public class Impact : Physic
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        entity.health.TakeLifeDamage(Random.Range(0, 0.5f)*amount);
    }
}

/// <summary>
/// 
/// </summary>
public class Perforation : Physic
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        //entity.health.TakeRegenDamage();
        var aux = 3 / Random.Range(1, 4);

        entity.health.TakeRegenDamage(aux*amount);
    }
}

public interface Init
{
    void Init();
}
