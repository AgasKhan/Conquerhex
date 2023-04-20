using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Cut")]
public class Cut : AbilityBase
{
    private bool isCuting = false; 

    private float cutRange;
    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        cooldownEnd.Set(3.5f, true);

        if (cooldownEnd.current == 0)
        {

            isCuting = true; 

        }

    }

    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        dir = dir.normalized; 

        InternalAttack(caster, dir, damages);

        weapon.Durability(5);
    }

    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        isCuting = false;

        cooldownEnd.Start();

        cooldownEnd.SubsDeltaTime();
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        if (isCuting && direction == direction.normalized)
        {
            RaycastHit[] hits = Physics.SphereCastAll(caster.transform.position, cutRange, direction, 0f);

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
