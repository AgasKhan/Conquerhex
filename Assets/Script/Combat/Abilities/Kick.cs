using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Kick")]
public class Kick : AbilityBase
{
    public override void ControllerDown(Vector2 dir, float button, Weapon weapon, bool cooldownEnd)
    {
        
    }

    public override void ControllerPressed(Vector2 dir, float button, Weapon weapon, bool cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    public override void ControllerUp(Vector2 dir, float button, Weapon weapon, bool cooldownEnd)
    {
        throw new System.NotImplementedException();
    }

    protected override void InternalAttack(Vector2 direction, Weapon weapon)
    {
        Debug.Log(direction);
    }
}
