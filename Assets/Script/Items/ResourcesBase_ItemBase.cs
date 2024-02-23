using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Currency/Resource", fileName = "Resource")]
public class ResourcesBase_ItemBase : ItemBase
{
    public EntityBase structure;

    public ResourceType itemType;

    protected override System.Type SetItemType()
    {
        return typeof(Resources_Item);
    }
}

public enum ResourceType
{
    Other,
    Mineral,
    Gemstone,
    Resource,
    Equipment,
    Ability,
    Modules
}

/*

public abstract class Gemstone : ResourcesBase_ItemBase
{
}



[CreateAssetMenu(menuName = "Currency/Resource/gemstone", fileName = "Resource")]
public class GemstoneBase : Gemstone
{
}

*/