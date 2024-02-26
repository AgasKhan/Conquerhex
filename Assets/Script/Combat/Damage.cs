using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDamageable
{
    void TakeDamage(ref Damage dmg);
}

[System.Serializable]
public struct Damage
{
    public string name;
    public float amount;
    public ClassDamage typeInstance;

    public override string ToString()
    {
        return amount.ToString().RichTextColor(typeInstance.color);
    }

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
public abstract class ElementalDamage : ClassDamage
{
}

/// <summary>
/// este es el daño fisico
/// </summary>
public abstract class PhysicalDamage : ClassDamage
{
}

public class DamageContainer : HybridArray<Damage>
{
    public DamageContainer(Func<Damage[]> staticArray) : base(staticArray)
    {
    }
}