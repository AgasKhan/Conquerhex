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

    HashSet<EntityBase> objectCasted = new HashSet<EntityBase>();

    public override void Init(Ability ability)
    {
        base.Init(ability);

        foreach (var item in castingActionBase.objectToCast)
        {
            objectCasted.Add(item.flyweight);
        }
    }

    public override IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = false;
        showParticleDamaged = false;

        DestructibleObjects obj = null;

        foreach (var item in entities)
        {
            if(item is DestructibleObjects && objectCasted.Contains(item.flyweight))
            {
                obj = (DestructibleObjects)item;
                obj.transform.position = caster.transform.position;
                break;
            }
        }

        if(obj == null)
        {
            obj = GameObject.Instantiate(castingActionBase.objectToCast[index], caster.transform.position, Quaternion.identity);
            obj.team = caster.container.team;

            index++;
            if (castingActionBase.objectToCast.Length <= index)
                index = 0;
        }

        obj.Teleport(caster.container.HexagoneParent,0);

        End = true;

        return new DestructibleObjects[] { obj };
    }
}