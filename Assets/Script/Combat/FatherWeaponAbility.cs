using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ItemBase where T : FatherWeaponAbility<T>
{
    [Space]

    [Header("Estadisticas")]

    [Header("Para habilidades: modificadores\nPara armas: valores por defecto\nExcepto velocidad")]
    [Tooltip("cooldown")]
    public float velocity;
    [Tooltip("rango de deteccion")]
    public float range;    

}


