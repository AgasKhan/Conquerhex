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
    void InternalTakeDamage(ref Damage dmg);
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
        all, life, regen, maxLife, maxRegen
    }
}


/*
 
Puro - padre de todos los tipos de danio - en su forma pura hace el danio de forma comun y corriente

Fisico - padre de los danios fisicos - en su forma pura hace danio solo a la vida

Elemental - padre de los danios elementales - en su forma pura hace danio solo a la regeneracion

DebuffLife - danio interno solo destinado a actuar como debufo - en su forma pura resta la cantidad maxima de vida

DebuffRegen - danio interno solo destinado a actuar como debufo - en su forma pura resta la cantidad maxima de regeneracion

//////////////////////////////////////////////////////////////

Fisicos:
Impactante - realiza mas danio a la vida en una relacion inversamente proporcional a la cantidad de regeneracion del enemigo
Cortante - realiza 0.1 de danio a la regeneracion por segundo, por la cantidad de danio % realizado a la vida
Perforante - realiza mas danio a la regeneracion en una realacion inversamente proporcional a la cantidad de salud del enemigo (1% minimo)

Elementales:
Frio - El daño que haces originalmente a la vida, es el que le quitas a la cantidad máxima de vida en la barra 
Calor- Te quita la mitad de la vida que te falta tanto en la vida como en la regeneracion


*/



//bioma desertico   * ganancia +25%
//bioma nevado      * ganancia -25%

//parry energia minima necesaria para castear 25
//de ser exitoso 75
//con calor 100
//con frio  50

//de fracasar 0
