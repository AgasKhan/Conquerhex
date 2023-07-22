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

    public override Pictionarys<string, string> GetDetails()
    {
        Pictionarys<string, string> aux = base.GetDetails();

        aux.Add("Maximum Life ", life.ToString());
        aux.Add("Maximum Regeneration ", regen.ToString());

        aux.Add("Damages Multipliers", vulnerabilities.ToString(" x ", "\n"));
        /*
        if (vulnerabilities.Length > 0)
        {
            aux.Add("Damages Multipliers", vulnerabilities.ToString(" x ", "\n"));
        }
        */
        return aux;
    }
}


public class StructureDiagram : Item<StructureBase>
{
    public override void Init(params object[] param)
    {

    }
}