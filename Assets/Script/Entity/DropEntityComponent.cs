using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class DropEntityComponent : ComponentOfContainer<Entity>
{
    DropBase dropBase;

    void Drop()
    {
        if (dropBase == null)
            return;

        for (int i = 0; i < dropBase.drops.Count; i++)
        {
            DropItem dropItem = dropBase.drops[i];

            var rng = dropItem.maxMinDrops.RandomPic();

            for (int ii = 0; ii < rng; ii++)
            {
                PoolManager.SpawnPoolObject(Vector2Int.zero, 
                    out RecolectableItem reference, 
                    transform.position + (Random.insideUnitCircle * 1.2f).Vec2to3(0), 
                    Quaternion.identity, 
                    container?.hexagoneParent?.transform);

                reference.Init(dropItem.item);
            }
        }
    }

    public override void OnEnterState(Entity param)
    {
        dropBase = param.flyweight?.GetFlyWeight<DropBase>();
        param.health.death += Drop;
    }

    public override void OnStayState(Entity param)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnExitState(Entity param)
    {
        param.health.death -= Drop;
    }
}