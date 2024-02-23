using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/HealthBase", fileName = "new HealthBase")]
public class HealthBase : EntityBase.FlyWeight
{
    [Header("Vida")]
    public float life;

    public float regen;

    [Header("Defensa")]

    public Damage[] vulnerabilities;
}
