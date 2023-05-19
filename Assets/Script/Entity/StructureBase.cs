using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/structure", fileName = "new Structure")]
public class StructureBase : ItemBase
{
    [Header("Vida")]
    public float life;

    public float regen;

    [Header("Defensa")]

    public Damage[] vulnerabilities;

    protected override void SetCreateItemType()
    {
        _itemType = typeof(StructureDiagram);
    }
}


public class StructureDiagram : Item<StructureBase>
{
    public override void Init(params object[] param)
    {

    }
}