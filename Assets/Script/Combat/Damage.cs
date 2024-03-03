using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public interface IDamageable
{
    /// <summary>
    /// Internal manage damage
    /// </summary>
    /// <param name="dmg"></param>
    void TakeDamage(ref Damage dmg);
}

[System.Serializable]
public struct Damage
{
    public string name;
    public float amount;
    public float knockBack;
    public DamageTypes.PureDamage typeInstance;

    static SuperDickLite<DamageTypes.PureDamage> damagesTypes = new SuperDickLite<DamageTypes.PureDamage>();

    public override string ToString()
    {
        return amount.ToString().RichTextColor(typeInstance.color);
    }

    public void ActionInitialiced(Entity go)
    {
        typeInstance.IntarnalAction(go, amount);
    }

    public static Damage Create<T>(float amount, float knockBack = 0, string name = "") where T : DamageTypes.PureDamage
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
            .GroupBy(dmg => dmg.typeInstance.IsParent)
            ;


        var proccesParents = procces.Where(group => group.Key).SelectMany(group=>group).ToArray();

        return procces
            .Where(group => !group.Key)
            .SelectMany(group => group)
            .Select(
                (dmg)=> 
                {
                    foreach (var modifier in proccesParents)
                    {
                        if (dmg.GetType().IsAssignableFrom(modifier.GetType()))
                        {
                            dmg = fusion(dmg, modifier);
                        }
                    }

                    return dmg;
                });

        /*
        foreach (var dmg in proccesChildrens)
        {
            foreach (var modifier in proccesParents)
            {
                if(dmg.GetType().IsAssignableFrom(modifier.GetType()))
                {
                    dmg = fusion(dmg, modifier);
                }
            }
        }
        */


    }

    public static DamageTypes.PureDamage GetFlyWeight<T>() where T : DamageTypes.PureDamage
    {
        if(damagesTypes.Count==0)
        {
            var dic = Manager<ShowDetails>.SearchByType<DamageTypes.PureDamage>();

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
    public enum Target
    {
        all, life, regen
    }

    [CreateAssetMenu(menuName = "Weapons/PureDamage", fileName = "PureDamage")]
    /// <summary>
    /// Clase padre de los tipos de danio
    /// </summary>
    public class PureDamage : ShowDetails
    {
        public Color color;

        public bool IsParent = false;

        public Target target = Target.all;

        public override string nameDisplay => this.GetType().Name;

        public virtual void IntarnalAction(Entity go, float amount)
        {
        }
    }

    [CreateAssetMenu(menuName = "Weapons/ElementalDamage", fileName = "ElementalDamage")]
    /// <summary>
    /// este es un da�o elemental
    /// </summary>
    public class ElementalDamage : PureDamage
    {
    }

    [CreateAssetMenu(menuName = "Weapons/PhysicalDamage", fileName = "PhysicalDamage")]
    /// <summary>
    /// este es el da�o fisico
    /// </summary>
    public class PhysicalDamage : PureDamage
    {
    }
}

