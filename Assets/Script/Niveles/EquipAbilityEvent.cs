using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipAbilityEvent : LogicActive<TutorialScenaryManager>
{
    public List<AbilityToEquip> myAbilities = new List<AbilityToEquip>();
    public override void Activate(TutorialScenaryManager specificParam)
    {
        foreach (var item in myAbilities)
        {
            specificParam.SetPlayerAbility(item);
        }
    }
}
