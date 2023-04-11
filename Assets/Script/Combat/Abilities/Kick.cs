using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Kick")]
public class Kick : AbilityBase
{
    public override void ControllerDown(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {
        
    }

    public override void ControllerPressed(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {
    }

    public override void ControllerUp(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {

    }

    protected override void InternalAttack(Vector2 direction, WeaponBase weapon)
    {

    }
}
