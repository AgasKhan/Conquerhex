using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class Stab : AbilityBase
{
    //Cuandos
    //Antes
    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        //Pocision en la mano del pj sin moverse ni extenderse
        //dir.y = 0;
        //dir.x = 0;
    }

    //Durante
    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        //Posicion extendida, moviendose en horizontal al apuñalar 
        //dir.x = 1;
        //dir.y = 0;

        weapon.Durability();
    }

    //Despues
    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        //Vuelve a posicion inicial
        //dir.x = 0;
        //dir.y = 0;

        //comienza a bajar el cooldown

    }

    //Como
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        Debug.Log(direction);

        direction.x = 1;
        direction.y = 0;



    }
}
