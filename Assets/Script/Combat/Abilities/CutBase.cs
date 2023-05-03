using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cut")]
public class CutBase : WeaponKataBase
{
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position, (tr) => { return caster != tr; });

        Damage(ref damages, caster, aux.ToArray());
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Cut);
    }
}

public class Cut : WeaponKata
{

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, CUT");

        

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeOnOff reference, caster.transform.position);

        this.reference = reference;

        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.radius;        
    }

    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        Debug.Log("estas manteniendo ataque 1, CUT");
    }

    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        Debug.Log("Soltaste ataque 1, CUT");

        //comienza a bajar el cooldown

        weapon.Durability(5);

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        //PoolManager.SpawnPoolObject(itemBase.indexParticles[0], caster.transform.position);

        
        reference.Off();
    }
}
