using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBuilding : InteractAction<Building>
{
    Building building;
    public override void Activate(Building specificParam)
    {
        specificParam.UpgradeLevel();
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        building = (Building)interactComp.container;
    }

    public override void ShowMenu(Character character)
    {
        if (building.currentLevel < building.maxLevel)
        {
            interactComp.genericMenu.detailsWindow.SetTexts(building.flyweight.nameDisplay + " Nivel " + building.currentLevel, $"En el siguiente nivel se desbloquean: {building.rewardNextLevel}\nRequisitos para el siguiente nivel: \n" + building.upgradesRequirements[building.currentLevel].GetRequiresString(character.inventory));
            interactComp.genericMenu.detailsWindow.SetImage(null);
            interactComp.genericMenu.CreateButton("Mejorar a nivel " + (building.currentLevel + 1).ToString(), () => building.PopUpAction(()=>Activate(building))).button.interactable = building.upgradesRequirements[building.currentLevel].CanCraft(character.inventory);
        }
        else
        {
            interactComp.genericMenu.detailsWindow.SetTexts(building.flyweight.nameDisplay + " Nivel Máximo", "\nHas llegado al nivel máximo de esta estructura\n\n");
            interactComp.genericMenu.detailsWindow.SetImage(null);
            interactComp.genericMenu.DestroyLastButtons();
        }


        
    }
}