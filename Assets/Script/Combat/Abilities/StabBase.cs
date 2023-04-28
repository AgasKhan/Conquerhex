using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class StabBase : WeaponKataBase
{
    //Como se efectua la habilidad
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position, (tr) => {return caster != tr; });

        Damage(ref damages, caster, aux);

        
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Stab);
    }
}

public class Stab : WeaponKata
{
    FadeOnOff reference;
    protected override void InternalControllerDown( Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, STAB");

        
        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out reference, caster.transform.position);
        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.radius;


    }

    //Durante, al mantener y moverlo
    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        Debug.Log("estas manteniendo ataque 1, STAB");
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        Debug.Log("Soltaste ataque 1, STAB");

        //comienza a bajar el cooldown

        weapon.Durability();

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        PoolManager.SpawnPoolObject(indexParticles[0], caster.transform.position);

        
        reference.Off();
    }
}
