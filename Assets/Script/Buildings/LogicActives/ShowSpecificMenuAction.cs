using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSpecificMenuAction : LogicActive<(InteractEntityComponent interact, Character character)>
{
    public InteractAction myIteractSpecific;

    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        myIteractSpecific.InteractInit(genericParams.interact);
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false);
        myIteractSpecific.ShowMenu(genericParams.character);
    }
}
