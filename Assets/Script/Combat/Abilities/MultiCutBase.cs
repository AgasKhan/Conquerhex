using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/MultiCut")]
public class MultiCutBase : WeaponKataBase
{
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.AreaWithRay(caster.transform.position, caster.transform.position, (algo) => { return caster != algo; }, (tr) => { return caster.transform == tr; });

        Damage(ref damages, aux);
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(MultiCut);
    }
}

public class MultiCut : WeaponKata
{
    FadeOnOff reference;

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, MULTICUT");

        if (caster.CompareTag("Player"))
        {
            var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out reference, caster.transform.position);
            aux.SetParent(caster.transform);

            aux.localScale *= itemBase.detect.radius;

        }
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

        var aux = PoolManager.SpawnPoolObject(indexParticles[0], caster.transform.position);

        aux.SetParent(caster.transform);

        if (caster.CompareTag("Player"))
            reference.Off();
    }
}
