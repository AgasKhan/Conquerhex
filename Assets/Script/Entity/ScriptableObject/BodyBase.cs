using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/body", fileName = "new Body")]
public class BodyBase : FlyWeight<EntityBase>
{
    [Header("Estadisiticas")]

    public float velocity;

    public float weightCapacity;

    public float stunTime;
}


