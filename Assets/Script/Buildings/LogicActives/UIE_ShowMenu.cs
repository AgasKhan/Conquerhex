using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIE_ShowMenu : LogicActive<(InteractEntityComponent interact, Character character)>
{
    public CraftingBuild craftBuild;

    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false);
        craftBuild.ShowMenu();
    }
}