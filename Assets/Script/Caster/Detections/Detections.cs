using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Detections : ShowDetails
{
    [SerializeField]
    public Detect<IGetEntity> detect;

    public List<Entity> Detect(ref List<Entity> bufferDetects, Entity caster, Vector2 direction, int numObjectives, float minRange, float maxRange, float dot)
    {
        InternalDetect(caster, direction, numObjectives, minRange, maxRange, dot).ToEntity(ref bufferDetects);

        return bufferDetects;
    }

    public abstract List<IGetEntity> InternalDetect(Entity caster, Vector2 direction, int numObjectives, float minRange,float maxRange, float dot);
}























