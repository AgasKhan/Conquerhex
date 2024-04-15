using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Pursuit
{

    protected override Vector3 InternalCalculate(MoveAbstract target)
    {
        return base.InternalCalculate(target) * -1;
    }
}
