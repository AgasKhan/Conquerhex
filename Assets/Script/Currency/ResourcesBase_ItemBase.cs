using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Currency/Item", fileName = "Item")]
public class ResourcesBase_ItemBase : ItemBase
{
    protected override void SetCreateItemType()
    {
        _itemType = typeof(Resources_Item);
    }
}
