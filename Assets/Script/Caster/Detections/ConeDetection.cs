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
    [SerializeField]
    bool withRay = true;
    public override List<IGetEntity> InternalDetect(Entity caster, Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
    {
        if (withRay)
            return detect.ConeWithRay(caster.transform, direction, (entity) => (entity.GetEntity() != null && entity.GetEntity().team != caster.team), numObjectives, minRange, maxRange, dot);
        else
            return detect.Cone(caster.transform.position, direction, (entity) => (entity.GetEntity() != null && entity.GetEntity().team != caster.team), detect.minDetects,numObjectives, minRange, maxRange, dot);
    }
}