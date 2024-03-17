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

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Damage))
            return ((Damage)obj).name == name;
        else
            return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void ActionInitialiced(Entity go, float amount)
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

    public static IEnumerable<Damage> Combine(Fusion fusion,  params IEnumerable<Damage>[] damages)
    {
        return Combine(AdditiveFusion, fusion, fusion, damages);
    }

    public static IEnumerable<Damage> Combine(Fusion sameFusion, Fusion combineFusion, Fusion parentFusion, params IEnumerable<Damage>[] damages)
    {
        var procces = 
            InternalCombine
            (combineFusion, damages
                .SelectMany
                (
                    dmg =>
                    {
                        return InternalCombine(sameFusion, dmg);
                    }
                )
            )
            .GroupBy(dmg => dmg.typeInstance.IsParent)
            .ToArray()
            ;


        var proccesParents = procces.Where(group => group.Key).SelectMany(group=>group);

        return procces
            .Where(group => !group.Key)
            .SelectMany(group => group)
            .Select(
                dmg=> 
                {
                    foreach (var modifier in proccesParents)
                    {
                        if (dmg.GetType().IsAssignableFrom(modifier.GetType()))
                        {
                            dmg = parentFusion(dmg, modifier);
                        }
                    }

                    return dmg;
                });
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

    static IEnumerable<Damage> InternalCombine(Fusion fusion,IEnumerable<Damage> collection)
    {
        return collection
                    .GroupBy(dmg => dmg.typeInstance)
                    .Select
                    (group => 
                        group.Aggregate(
                        (dmgSum, dmg) =>
                        {
                                dmgSum = fusion(dmgSum, dmg);
                                return dmgSum;
                        })
                    );
    }
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
}

