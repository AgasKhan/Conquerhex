using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KataBase que tiene configurado su ataque interno en la deteccion de un cono en la posicion del caster
/// </summary>
[CreateAssetMenu(menuName = "Abilities/ConeDetection")]
[System.Serializable]
public class ConeDetection : Detections
{
    public override List<IGetEntity> InternalDetect(Entity caster, Vector3 pos ,Vector3 direction, System.Func<IGetEntity, bool> chck, int numObjectives, float minRange, float maxRange, float dot)
    {
        if (withRay)
            return detect.ConeWithRay(pos, direction, chck, numObjectives, minRange, maxRange, dot);
        else
            return detect.Cone(pos, direction, chck, detect.minDetects,numObjectives, minRange, maxRange, dot);
    }
}