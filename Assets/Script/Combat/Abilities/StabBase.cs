using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class StabBase : WeaponKataBase
{
    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    //Cuandos
    //Antes, al apretar el boton
    

    //Como se efectua la habilidad
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position, (tr) => {return caster != tr; });

        Damage(ref damages, aux);

        
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Stab);
    }
}

public class Stab : WeaponKata
{
    protected override void InternalControllerDown( Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque 1, STAB");
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

        PoolManager.SpawnPoolObject(indexParticles[2], caster.transform.position);
    }
}
