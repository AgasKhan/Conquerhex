using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Persuit
{

    public override Vector2 Calculate(MoveAbstract target)
    {
        return base.Calculate(target) * -1;
    }
}
