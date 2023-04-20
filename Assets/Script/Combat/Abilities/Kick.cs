using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Kick")]
public class Kick : AbilityBase
{
    public override void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    public override void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    public override void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {

    }
}
