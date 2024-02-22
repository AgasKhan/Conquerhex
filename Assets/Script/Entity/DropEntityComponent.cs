using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEntityComponent : MonoBehaviour, IState<Entity>
{
    public List<DropItem> drops = new List<DropItem>();

    void Drop()
    {
        for (int i = 0; i < drops.Count; i++)
        {
            DropItem dropItem = drops[i];

            var rng = dropItem.maxMinDrops.RandomPic();

            for (int ii = 0; ii < rng; ii++)
            {
                PoolManager.SpawnPoolObject(Vector2Int.zero, out RecolectableItem reference, transform.position + new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-1.2f, 1.2f)));

                reference.Init(dropItem.item);
            }
        }
    }

    public void OnEnterState(Entity param)
    {
        param.health.death += Drop;
    }

    public void OnExitState(Entity param)
    {
        param.health.death -= Drop;
    }

    public void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
    }
}

[System.Serializable]
public struct DropItem
{
    public Pictionarys<int, int> maxMinDrops;

    public ResourcesBase_ItemBase item;
}