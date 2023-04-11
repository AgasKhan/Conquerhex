using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stab")]
public class Stab : AbilityBase
{
<<<<<<< Updated upstream
    //¿Cuando?
    public override void ControllerDown(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
=======
    public override void ControllerDown(Vector2 dir, float button, WeaponBase weapon, Timer cooldownEnd)
>>>>>>> Stashed changes
    {

    }

<<<<<<< Updated upstream
    //¿Cuando?
    public override void ControllerPressed(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
=======
    public override void ControllerPressed(Vector2 dir, float button, WeaponBase weapon, Timer cooldownEnd)
>>>>>>> Stashed changes
    {

    }

<<<<<<< Updated upstream
    //¿Cuando?
    public override void ControllerUp(Vector2 dir, float button, WeaponBase weapon, bool cooldownEnd)
=======
    public override void ControllerUp(Vector2 dir, float button, WeaponBase weapon, Timer cooldownEnd)
>>>>>>> Stashed changes
    {

    }

    //¿Como?
    protected override void InternalAttack(Vector2 direction, WeaponBase weapon)
    {
        Debug.Log(direction);
    }
}
