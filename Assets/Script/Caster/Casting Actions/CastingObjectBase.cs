using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Casting/CastingObject", fileName = "new CastingObject")]
public class CastingObjectBase : CastingActionBase
{
    public DestructibleObjects[] objectToCast;

    protected override Type SetItemType()
    {
        return typeof(CastingObject);
    }
}


public class CastingObject : CastingAction<CastingObjectBase>
{
    int index;

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = false;
        showParticleDamaged = false;

        var obj = GameObject.Instantiate(castingActionBase.objectToCast[index], caster.transform.position, Quaternion.identity);

        obj.team = caster.container.team;

        obj.Teleport(caster.container.hexagoneParent,0);

        index++;
        if (castingActionBase.objectToCast.Length <= index)
            index = 0;

        End = true;

        return null;
    }
}