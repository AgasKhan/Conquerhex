using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    Perforation
}


public abstract class ClassDamage
{
    static Dictionary<System.Type, ClassDamage> typesDamages = new Dictionary<System.Type, ClassDamage>();

    public ClassDamage()
    {
        typesDamages.Add(this.GetType(), this);

        Debug.Log("se creo: " + this.GetType().Name);
    }

    public static ClassDamage SearchDamage(EnumDamage type)
    {
        string nameClass = type.ToString();

        Debug.Log(nameClass);

        foreach (var item in typesDamages)
        {
            if (item.Key.Name == nameClass)
            {
                Debug.Log("encontro");
                return item.Value;
            }
        }

        Debug.Log("NO encontro");

        string completeNameClass = type.GetType().Namespace + "." + nameClass;

        return (ClassDamage)Activator.CreateInstance(Type.GetType(completeNameClass));
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
        entity.Effect(amount/3, 
            () =>
            {
                entity.health.TakeRegenDamage(Time.deltaTime);
            },
            null
            );
    }
}

/// <summary>
/// danio extra aleatorio de hasta el 50%
/// </summary>
public class Impact : Physic
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        entity.health.TakeLifeDamage(UnityEngine.Random.Range(0, 0.5f)*amount);
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
        var aux = 3 / UnityEngine.Random.Range(1, 4);

        entity.health.TakeRegenDamage(aux*amount);
    }
}

public interface Init
{
    void Init();
}
