using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Currency/Resource", fileName = "Resource")]
public class ResourcesBase_ItemBase : ItemBase
{
    public StructureBase structure;

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Resources_Item);
    }
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