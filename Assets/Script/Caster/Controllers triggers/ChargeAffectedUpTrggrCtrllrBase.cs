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
    new public ChargeAffectedUpTrggrCtrllrBase triggerBase => (ChargeAffectedUpTrggrCtrllrBase)base.triggerBase;

    public override List<Entity> InternalDetect(Entity caster,Vector3 dir, float timePressed = 0, float? minRange = null, float? maxRange = null, float? dot = null)
    {
        return ability.itemBase.Detect(ref ability.affected, caster.container, dir, (int)Mathf.Clamp(timePressed * triggerBase.multiplyTime, 1, ability.itemBase.maxDetects), minRange ?? FinalMinRange, maxRange ?? FinalMaxRange, dot ?? ability.itemBase.dot);
    }
}