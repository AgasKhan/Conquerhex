using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Crush")]
public class CrushBase : WeaponKataBase
{
    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    //OJO QUE ES UNA REFERENCIA PARA TODOS
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.AreaWithRay(caster.transform.position, caster.transform.position, (algo)=> { return caster != algo; } ,(tr) => { return caster.transform == tr; });

        Damage(ref damages, aux);
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Crush);
    }
}

public class Crush : WeaponKata
{
    FadeOnOff reference;

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, CRUSH");

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
        Debug.Log("estas manteniendo ataque 1, CRUSH");
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        Debug.Log("Soltaste ataque 1, CRUSH");

        //comienza a bajar el cooldown

        weapon.Durability(3);

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        PoolManager.SpawnPoolObject(indexParticles[0], caster.transform.position);

        if (caster.CompareTag("Player"))
            reference.Off();
    }
}