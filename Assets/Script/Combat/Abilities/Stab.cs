using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class Stab : AbilityBase
{
    public override void ControllerDown(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    public override void ControllerPressed(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    public override void ControllerUp(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    protected override void InternalAttack(Vector2 direction, WeaponBase weapon)
    {
        Debug.Log(direction);
    }
}
