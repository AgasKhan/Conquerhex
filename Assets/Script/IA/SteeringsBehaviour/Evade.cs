using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Pursuit
{

    protected override Vector2 InternalCalculate(MoveAbstract target)
    {
        return base.InternalCalculate(target) * -1;
    }
}
