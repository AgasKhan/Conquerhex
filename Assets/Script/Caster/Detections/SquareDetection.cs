using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KataBase que tiene configurado su ataque interno en la deteccion de un area en la posicion del caster
/// </summary>
[CreateAssetMenu(menuName = "Abilities/SquareDetection")]
[System.Serializable]
public class SquareDetection : Detections
{
    /// <summary>
    /// cuadrado enfrente del jugador
    /// </summary>
    /// <param name="caster">caster</param>
    /// <param name="direction">direccion</param>
    /// <param name="numObjectives">maximo de objetivos</param>
    /// <param name="minRange">area minima</param>
    /// <param name="maxRange">ancho</param>
    /// <param name="dot">maxima distancia</param>
    /// <returns></returns>
    public override List<IGetEntity> InternalDetect(Entity caster,Vector3 pos , Vector3 direction, System.Func<IGetEntity, bool> chck, int numObjectives, float minRange, float maxRange, float dot)
    {
        return detect.SquareWithRay(pos, direction,chck, numObjectives, minRange, maxRange, dot);
    }
}
