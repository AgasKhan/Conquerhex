using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Persuit
{

    protected override Vector2 InternalCalculate(MoveAbstract target)
    {
        return base.InternalCalculate(target) * -1;
    }
}
