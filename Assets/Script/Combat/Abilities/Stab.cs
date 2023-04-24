using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class Stab : AbilityBase
{
    private bool isStabbing = false; // Indica si el jugador está actualmente apuñalando
    
    private float stabRange;

    /*
    Entity caster: ENTIDAD QUE USA LA HABILIDAD
    Vector2 dir: HACIA DONDE APUNTA LA HABILIDAD
    float button: EL TIEMPO QUE MANTUVO PRESIONADO EL BOTON (No se usara en ControllerDown)
    Weapon weapon: EL ARMA EQUIPADA CON ESTA HABILIDAD
    Timer cooldownEnd: EL TIEMPO DE REUTILIZACION DE LA HABILIDAD
     */

    //Cuandos
    //Antes, al apretar el boton
    public override void ControllerDown (Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        cooldownEnd.Set(3f, true);

        if (cooldownEnd.current == 0)
        {

            isStabbing = true; // El jugador está preparándose para apuñala

        }

        Debug.Log("presionaste ataque 1");

    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        dir = dir.normalized; // Establecer la dirección del apuñalamiento como la dirección actual del controlador

        InternalAttack(caster, dir, damages);

        weapon.Durability();

        Debug.Log("estas manteniendo ataque 1");
    }

    //Despues, al sotarlo
    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        
        isStabbing = false;

        cooldownEnd.Start();

        cooldownEnd.SubsDeltaTime();
        //comienza a bajar el cooldown

        Debug.Log("Soltaste ataque 1");
    }

    //Como se efectua la habilidad
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        if (isStabbing && direction == direction.normalized)
        {
            RaycastHit[] hits = Physics.SphereCastAll(caster.transform.position, stabRange, direction, 0f);

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

