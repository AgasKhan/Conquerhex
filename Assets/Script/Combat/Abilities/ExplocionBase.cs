using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Explocion")]

public class ExplocionBase : WeaponKataBase
{
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.AreaWithRay(caster.transform.position, caster.transform.position, (algo) => { return caster.transform != algo; }, (tr) => { return caster.transform == tr; });

        Damage(ref damages, caster, aux);
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Explocion);
    }
}

public class Explocion : WeaponKata
{
    FadeOnOff reference;

    protected override void InternalControllerDown(Vector2 dir, float tim)
    {
        Debug.Log("presionaste ataque 1, EXPLOCION");

       
        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out reference, caster.transform.position);
        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.radius;


    }

    protected override void InternalControllerPress(Vector2 dir, float tim)
    {
        Debug.Log("estas manteniendo ataque 1, EXPLOCION");
    }

    protected override void InternalControllerUp(Vector2 dir, float tim)
    {
        Debug.Log("Soltaste ataque 1, EXPLOCION");

        //comienza a bajar el cooldown

        weapon.Durability(15);

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        var aux = PoolManager.SpawnPoolObject(indexParticles[0], caster.transform.position);

        aux.SetParent(caster.transform);

        
        reference.Off();
    }
}