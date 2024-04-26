using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMenuAction : LogicActive<(InteractEntityComponent interact, Character character)>
{
    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        genericParams.interact.genericMenu.Init();
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false);
        genericParams.interact.genericMenu.Create(genericParams.character);
    }
}
