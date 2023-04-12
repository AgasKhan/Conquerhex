using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class Stab : AbilityBase
{
    //cuandos

    public override void ControllerDown(Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {

    }


    public override void ControllerPressed(Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {

    }

    public override void ControllerUp(Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {

    }

    //como
    protected override void InternalAttack(Vector2 direction, Damage[] damages)
    {
        Debug.Log(direction);
    }
}
