using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/Casting/DestroyObject", fileName = "new DestroyObject")]
public class DestroyObjectBase : CastingActionBase
{
    protected override Type SetItemType()
    {
        return typeof(DestroyObjec);
    }
}

public class DestroyObjec : CastingAction<DestroyObjectBase>
{
    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = false;
        showParticleDamaged = false;

        DestructibleObjects obj = null;

        foreach (var item in entities)
        {
            if (item is DestructibleObjects)
            {
                obj = (DestructibleObjects)item;
                break;
            }
        }

        if (obj != null)
        {
            obj.hexagoneParent?.ExitEntity(obj);

            GameManager.eventQueueGamePlay.Enqueue(()=>GameObject.Destroy(obj.gameObject));
        }
        else
        {
            caster.positiveEnergy += ability.CostExecution;
        }

        End = true;

        return null;
    }
}
