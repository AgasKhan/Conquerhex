using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{

    protected override Vector3 InternalCalculate(MoveAbstract target)
    {
        return Direction(target);
    }
}
