using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KataBase que tiene configurado su ataque interno en la deteccion de un area en la posicion del caster
/// </summary>
public abstract class AreaKataBase : WeaponKataBase
{
    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack description", "Genera un area de ataque circular en base a un rango");

        return aux;
    }

    protected override Entity[] InternalAttack(Entity caster, Vector2 direction, Damage[] damages, int numObjectives, float range)
    {
        var pos = caster.transform.position + direction.Vec2to3(0) * detect.distance;

        var aux = detect.AreaWithRay(caster.transform, (entity) => { return caster != entity; }, numObjectives, range);

        Damage(ref damages, caster, aux.ToArray());

        return aux.ToArray();
    }
}

























