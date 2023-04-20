using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ItemBase where T : FatherWeaponAbility<T>
{
    [Space]

    [Header("Estadisticas")]
    public Damage[] damages = new Damage[1];
    public float velocity;
}


