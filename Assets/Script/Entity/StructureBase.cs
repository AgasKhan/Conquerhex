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
        Pictionarys<string, string> aux = new Pictionarys<string, string>();

        aux.Add("\nMaximum Life ", life.ToString());
        aux.Add("Maximum Regeneration ", regen.ToString());

        if(vulnerabilities.Length > 0)
        {
            aux.Add("\nDamages Multipliers ", "");

            aux.Add(vulnerabilities.ToString(" x ", ""), "\n");
            /*
            for (int i = 0; i < vulnerabilities.Length; i++)
            {
                aux.Add(vulnerabilities[i].typeInstance.name + " x ", vulnerabilities[i].amount.ToString());
            }*/
        }
        
        return aux;
    }
}


public class StructureDiagram : Item<StructureBase>
{
    public override void Init(params object[] param)
    {

    }
}