using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MultiCut")]
public class MultiCutBase : WeaponKataBase
{
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position, (algo) => { return caster != algo; });

        Damage(ref damages, caster, aux.ToArray());
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(MultiCut);
    }
}

public class MultiCut : WeaponKata
{
    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, MULTICUT");


        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeOnOff reference, caster.transform.position);

        this.reference = reference;
        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.diameter;


    }

    //Durante, al mantener y moverlo
    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        Debug.Log("estas manteniendo ataque 1, MULTICUT");
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        Debug.Log("Soltaste ataque 1, MULTICUT");

        //comienza a bajar el cooldown

        weapon.Durability(7);

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        /*
        var aux = PoolManager.SpawnPoolObject(itemBase.indexParticles[0], caster.transform.position);

        aux.SetParent(caster.transform);
        */
        
        reference.Off();
    }
}
