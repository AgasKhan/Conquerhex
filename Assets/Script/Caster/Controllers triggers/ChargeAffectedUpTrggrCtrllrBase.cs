using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/ChargeAffectedUpTriggerControllerBase")]
public class ChargeAffectedUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Multiplicador de tiempo el aumento de afectados")]
    public float multiplyTime=1;

    protected override System.Type SetItemType()
    {
        return typeof(ChargeAffectedUpTrggrCtrllr);
    }
}

public class ChargeAffectedUpTrggrCtrllr : UpTrggrCtrllr
{
    public override List<Entity> Detect(Vector2 dir, float timePressed = 0, float? range = null, float? dot = null)
    {
        return ability.itemBase.Detect(ref ability.affected, caster.container, dir, (int)Mathf.Clamp(timePressed * GetTrggrBs<ChargeAffectedUpTrggrCtrllrBase>().multiplyTime, 1, ability.itemBase.maxDetects), FinalRange, dot ?? ability.itemBase.dot);
    }
}