using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
    public override Vector2 Calculate(MoveAbstract target)
    {
        return Direction(target);
    }
}
