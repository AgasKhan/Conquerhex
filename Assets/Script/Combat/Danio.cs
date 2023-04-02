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
        typeInstance = DamageList.SearchDamage(type);
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


public abstract class FatherDamageAbilities<T>
{
    protected Pictionarys<string, T> types = new Pictionarys<string, T>();

    protected T SearchOrCreate(System.Enum type , Pictionarys<string, T> types)
    {
        string nameClass = type.ToString();

        foreach (var item in types)
        {
            if (item.key == nameClass)
            {
                return item.value;
            }
        }

        Debug.Log(nameClass);

        string completeNameClass = type.GetType().Namespace + "." + nameClass;

        var newAux = (T)Activator.CreateInstance(Type.GetType(completeNameClass));

        this.types.Add(newAux.GetType().Name, newAux);

        return newAux;
    }
}


public class DamageList : FatherDamageAbilities<ClassDamage>
{
    static DamageList instance;

    public static ClassDamage SearchDamage(System.Enum type)
    {
        if (instance == null)
            instance = new DamageList();

        var aux = instance.SearchOrCreate(type, instance.types);

        return aux;
    }
}

public abstract class ClassDamage 
{
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


