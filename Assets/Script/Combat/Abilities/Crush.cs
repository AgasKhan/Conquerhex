using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Crush")]
public class Crush : WeaponKataBase
{
    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    //OJO QUE ES UNA REFERENCIA PARA TODOS
    FadeOnOff reference;

    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles)
    {
        Debug.Log("presionaste ataque 1, CRUSH");

        if (caster.CompareTag("Player"))
        {
            var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out reference, caster.transform.position);
            aux.SetParent(caster.transform);

            aux.localScale *= detect.radius;

        }
    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles)
    {
        Debug.Log("estas manteniendo ataque 1, CRUSH");
    }

    //Despues, al sotarlo
    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles)
    {
        Debug.Log("Soltaste ataque 1, CRUSH");

        //comienza a bajar el cooldown

        weapon.Durability(3);

        cooldownEnd.Reset();

        Attack(caster, dir, weapon);

        PoolManager.SpawnPoolObject(particles[0], caster.transform.position);

        if (caster.CompareTag("Player"))
            reference.Off();
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.AreaWithRay(caster.transform.position, caster.transform.position, (algo)=> { return caster != algo; } ,(tr) => { return caster.transform == tr; });

        Damage(ref damages, aux);
    }
}
