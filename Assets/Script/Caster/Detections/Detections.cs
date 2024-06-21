using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Detections : ShowDetails
{
    public enum DetectionType
    {
        Same, ToAttack
    }

    [SerializeField]
    DetectionType type = DetectionType.ToAttack;

    [SerializeField]
    protected bool withRay = true;

    [SerializeField]
    public Detect<IGetEntity> detect;

    public List<Entity> Detect(ref List<Entity> bufferDetects, Entity caster, Vector3 pos ,Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
    {
        System.Func<IGetEntity, bool> chck;

        switch (type)
        {
            case DetectionType.Same:
                chck = (entity) => (entity.GetEntity() != null && caster.team == entity.GetEntity().team);
                break;
            case DetectionType.ToAttack:
                chck = (entity) => (entity.GetEntity() != null && caster.team.TeamAttack(entity.GetEntity().team));
                break;

            default:
                chck = null;
                break;
        }

        /*
        List<IGetEntity> result = null;


        switch (type)
        {
            case DetectionType.Area:

                if (withRay)
                    result = detect.AreaWithRay(pos, chck, numObjectives, minRange, maxRange);
                else
                    result = detect.Area(pos, chck, detect.minDetects, numObjectives, minRange, maxRange);

                break;

            case DetectionType.Cone:

                if (withRay)
                    result = detect.ConeWithRay(pos, direction, chck, numObjectives, minRange, maxRange, dot);
                else
                    result = detect.Cone(pos, direction, chck, detect.minDetects, numObjectives, minRange, maxRange, dot);

                break;

            case DetectionType.Square:

                if (withRay)
                    result = detect.SquareWithRay(pos, direction, chck, numObjectives, minRange, maxRange, dot);
                else
                    result = detect.Square(pos, direction, chck, detect.minDetects, numObjectives, minRange, maxRange, dot);

                break;

            default:
                break;
        }
        result.ToEntity(ref bufferDetects);

        */
        
        InternalDetect(caster, pos, direction, chck, numObjectives, minRange, maxRange, dot).ToEntity(ref bufferDetects);


        return bufferDetects;
    }

    public abstract List<IGetEntity> InternalDetect(Entity caster, Vector3 pos, Vector3 direction, System.Func<IGetEntity, bool> chck, int numObjectives, float minRange,float maxRange, float dot);    
}

























