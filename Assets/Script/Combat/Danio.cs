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


public abstract class FatherDamageAbilities<T> : ScriptableObject
{
    protected Dictionary<System.Type, T> types = new Dictionary<System.Type, T>();

    protected T SearchOrCreate(System.Enum type , Dictionary<System.Type, T> types)
    {
        string nameClass = type.ToString();

        Debug.Log(nameClass);

        foreach (var item in types)
        {
            if (item.Key.Name == nameClass)
            {
                Debug.Log("encontro");
                return item.Value;
            }
        }

        Debug.Log("NO encontro");

        string completeNameClass = type.GetType().Namespace + "." + nameClass;

        var newAux = (T)Activator.CreateInstance(Type.GetType(completeNameClass));

        this.types.Add(typeof(T), newAux);

        return newAux;
    }


}

[CreateAssetMenu(menuName = "Managers/DamageList", fileName = "DamageList")]
public class DamageList : FatherDamageAbilities<ClassDamage>
{
    static DamageList instance;

    [SerializeReference]
    List<string> names = new List<string>();

    public static ClassDamage SearchDamage(System.Enum type)
    {
        var aux = instance.SearchOrCreate(type, instance.types);

        instance.names.Add(aux.GetType().Name);

        return aux;
    }

    private void Awake()
    {
        instance = this;
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

public interface Init
{
    void Init();
}
