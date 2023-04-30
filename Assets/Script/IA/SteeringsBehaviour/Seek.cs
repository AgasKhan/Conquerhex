using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{

    [field: SerializeField, Range(0f, 2.5f)]
    public float SeekWeight { get; private set; }

    public override Vector2 Calculate(MoveAbstract target)
    {
        return Direction(target) * SeekWeight;
    }
}
