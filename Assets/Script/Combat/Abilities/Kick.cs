using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Kick")]
public class Kick : AbilityBase
{
    private bool isKicking = false;

    private float kickRange;

    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        cooldownEnd.Set(5f, true);

        if (cooldownEnd.current == 0)
        {

            isKicking = true; 

        }

    }

    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        dir = dir.normalized;

        InternalAttack(caster, dir, damages);

        weapon.Durability(3);
    }

    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        isKicking = true;

        cooldownEnd.Start();

        cooldownEnd.SubsDeltaTime();
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        if (isKicking && direction == direction.normalized)
        {
            RaycastHit[] hits = Physics.SphereCastAll(caster.transform.position, kickRange, direction, 0f);

            foreach (RaycastHit hit in hits)
            {
                Health health = hit.collider.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    foreach (Damage damage in damages)
                    {

                        //Entity.TakeDamage(damage);
                    }
                }
            }
        }
    }
}
