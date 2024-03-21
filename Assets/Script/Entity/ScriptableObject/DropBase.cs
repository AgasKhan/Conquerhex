using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/Drop", fileName = "New Drop")]
public class DropBase : FlyWeight<EntityBase>
{
    public List<DropItem> drops = new List<DropItem>();
}

[System.Serializable]
public struct DropItem
{
    public Pictionarys<int, int> maxMinDrops;

    public ResourcesBase_ItemBase item;
}