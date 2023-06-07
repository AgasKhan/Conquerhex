using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ItemBase where T : FatherWeaponAbility<T>
{
    [Space]

    [Header("Estadisticas")]

    [Header("Para habilidades: modificadores\nPara armas: valores por defecto\nExcepto velocidad")]
    [Tooltip("velocidad de ejecusion")]
    public float velocity;
    [Tooltip("rango de deteccion")]
    public float range;
    [Tooltip("array de danios")]
    public Damage[] damages = new Damage[1];

}


