using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stab : FunctionSlot
{
    public override void InternalAttack(Vector2 direction, Weapon weapon)
    {
        Debug.Log(direction);
    }
}
