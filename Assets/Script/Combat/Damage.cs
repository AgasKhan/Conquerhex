using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public interface IDamageable
{
    void TakeDamage(ref Damage dmg);
}

[System.Serializable]
public struct Damage
{
    public string name;
    public float amount;
    public float knockBack;
    public DamageTypes.ParentDamage typeInstance;

    static SuperDickLite<DamageTypes.ParentDamage> damagesTypes = new SuperDickLite<DamageTypes.ParentDamage>();

    public override string ToString()
    {
        return amount.ToString().RichTextColor(typeInstance.color);
    }

    public void ActionInitialiced(Entity go)
    {
        typeInstance.IntarnalAction(go, amount);
    }

    public static Damage Create<T>(float amount, float knockBack = 0, string name = "") where T : DamageTypes.ParentDamage
    {
        Damage dmg = new Damage();

        dmg.amount = amount;

        dmg.name = name;

        dmg.knockBack = knockBack;

        dmg.typeInstance = GetFlyWeight<T>();

        return dmg;
    }

    public static Damage MultiplicativeFusion(Damage original, Damage toCompare)
    {
        original.amount *= toCompare.amount;

        return original;
    }

    public static Damage AdditiveFusion(Damage original, Damage toCompare)
    {
        original.amount += toCompare.amount;

        return original;
    }

    public static IEnumerable<Damage> Combine(Fusion fusion, params IEnumerable<Damage>[] damages)
    {
        var procces = damages
            .SelectMany(dmg => dmg)
            .GroupBy(dmg => dmg.typeInstance)
            .Select(group => group
                .Aggregate
                (
                    (dmgSum, dmg) =>
                    {
                        dmgSum = fusion(dmgSum, dmg);
                        return dmgSum;
                    }
                ))
            //.GroupBy(dmg => dmg.typeInstance.IsParent)
            ;

        //var process = procces.Where(group => group.Key).SelectMany(group=>group);

        //procces.Where(group=>!group.Key).SelectMany(group=>group).Join(process, (pr)=> { return new Damage(); }, )

        return procces;
    }

    public static DamageTypes.ParentDamage GetFlyWeight<T>() where T : DamageTypes.ParentDamage
    {
        if(damagesTypes.Count==0)
        {
            var dic = Manager<ShowDetails>.SearchByType<DamageTypes.ParentDamage>();

            foreach (var item in dic)
            {
                damagesTypes[item.value.GetType()] = item.value;
            }
        }

        return damagesTypes[typeof(T)];
    }

    public delegate Damage Fusion(Damage original, Damage toCompare);
}

public class DamageContainer : HybridArray<Damage>
{
    public DamageContainer(Func<Damage[]> staticArray) : base(staticArray)
    {
    }
}


namespace DamageTypes
{
    /// <summary>
    /// Clase padre de los tipos de danio
    /// </summary>
    public abstract class ParentDamage : ShowDetails
    {
        public Color color;

        public bool IsParent = false;

        public override string nameDisplay => this.GetType().Name;

        public abstract void IntarnalAction(Entity go, float amount);
    }

    /// <summary>
    /// este es un daño elemental
    /// </summary>
    public class ElementalDamage : ParentDamage
    {
        public override void IntarnalAction(Entity go, float amount)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// este es el daño fisico
    /// </summary>
    public class PhysicalDamage : ParentDamage
    {
        public override void IntarnalAction(Entity go, float amount)
        {
            throw new NotImplementedException();
        }
    }
}

