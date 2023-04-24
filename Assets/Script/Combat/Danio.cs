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
    public ClassDamage typeInstance;

    public void ActionInitialiced(Entity go)
    {
        typeInstance.IntarnalAction(go, amount);
    }
}

public abstract class ClassDamage : ShowDetails
{
    public Color color;
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
public abstract class PhysicalDamage : ClassDamage
{
}
