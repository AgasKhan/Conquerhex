using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEvent : LogicActive<TutorialScenaryManager>
{
    public Ingredient weaponForPlayer;
    public override void Activate(TutorialScenaryManager specificParam)
    {
        specificParam.GiveToPlayer(weaponForPlayer.Item, weaponForPlayer.Amount);
    }
}
