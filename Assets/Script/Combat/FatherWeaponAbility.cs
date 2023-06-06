using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ItemBase where T : FatherWeaponAbility<T>
{
    [Space]

    [Header("Estadisticas")]
    public Damage[] damages = new Damage[1];

    [Header("Para habilidades: valores por defecto\nPara armas: modificadores")]
    [Tooltip("velocidad de ejecusion")]
    public float velocity;
    [Tooltip("rango de deteccion")]
    public float range;
}


