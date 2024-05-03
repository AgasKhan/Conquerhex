using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KataBase que tiene configurado su ataque interno en la deteccion de un area en la posicion del caster
/// </summary>
[CreateAssetMenu(menuName = "Abilities/AreaDetection")]
[System.Serializable]
public class AreaDetection : Detections
{
    public override List<IGetEntity> InternalDetect(Entity caster ,Vector3 pos , Vector3 direction, System.Func<IGetEntity, bool> chck, int numObjectives, float minRange, float maxRange, float dot)
    {
        if(withRay)
            return detect.AreaWithRay(pos, 
                chck, 
                numObjectives, 
                minRange, 
                maxRange);
        else
            return detect.Area(pos, 
                chck, 
                detect.minDetects ,
                numObjectives, 
                minRange, 
                maxRange);
    }
}