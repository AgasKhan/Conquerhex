using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Seek
{

    [field: SerializeField, Range(0f, 2.5f)]
    public float FleeWeight { get; private set; }
    public override Vector2 Calculate(MoveAbstract target)
    {
        return base.Calculate(target) *-1 * FleeWeight;
    }
}
