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
    public override List<IGetEntity> InternalDetect(Entity caster, Vector2 direction, int numObjectives, float minRange, float maxRange, float dot)
    {
        return detect.AreaWithRay(caster.transform, (entity) => (entity.GetEntity() != null && entity.GetEntity().team != caster.team), numObjectives, minRange, maxRange);
    }
}