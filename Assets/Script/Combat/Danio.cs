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
        typeInstance = Manager<ClassDamage>.pic.SearchOrCreate(type.ToString());
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


