using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cut")]
public class Cut : AbilityBase
{
    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        Debug.Log("presionaste ataque 1, CUT");
    }

    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        Debug.Log("estas manteniendo ataque 1, CUT");
    }

    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        Debug.Log("Soltaste ataque 1, CUT");

        //comienza a bajar el cooldown

        weapon.Durability(5);

        if (cooldownEnd.Chck)
        {
            cooldownEnd.Reset();

            Attack(caster, dir, weapon);
        }
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position, (tr) => { return caster.transform != tr; });

        Damage(ref damages, aux);
    }
}
