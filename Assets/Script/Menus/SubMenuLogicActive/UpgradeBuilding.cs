using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBuilding : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        var aux = specificParam[0];
        if (aux.currentLevel < aux.maxLevel)
        {
            aux.myBuildSubMenu.detailsWindow.SetTexts(aux.structureBase.nameDisplay + " Nivel " + aux.currentLevel, $"En el siguiente nivel se desbloquean: {aux.rewardNextLevel}\nRequisitos para el siguiente nivel: \n" + aux.upgradesRequirements[aux.currentLevel].GetRequiresString());
            aux.myBuildSubMenu.detailsWindow.SetImage(null);
            aux.myBuildSubMenu.CreateButton("Mejorar a nivel " + (aux.currentLevel + 1).ToString(), aux.UpgradeLevel);
            //aux.myBuildSubMenu.CreateButton("Mejorar a nivel " + (aux.currentLevel+1).ToString(), ()=>aux.PopUpAction(aux.UpgradeLevel));
        }
        else
        {
            aux.myBuildSubMenu.detailsWindow.SetTexts(aux.structureBase.nameDisplay + " Nivel Máximo", "\nHas llegado al nivel máximo de esta estructura\n");
            aux.myBuildSubMenu.detailsWindow.SetImage(null);
            aux.myBuildSubMenu.DestroyCraftButtons();
        }
            
    }

    void RefreshUpgrade(Building aux)
    {

    }
}
