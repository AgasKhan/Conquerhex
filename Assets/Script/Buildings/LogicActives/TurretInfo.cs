using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInfo : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        var aux = specificParam[0];

        aux.myBuildSubMenu.detailsWindow.SetTexts("", aux.flyweight.GetDetails()["Description"]);

        if (aux.currentLevel == 0)
            aux.myBuildSubMenu.detailsWindow.SetImage(((TurretBuild)aux).baseSprite);
        else
            aux.myBuildSubMenu.detailsWindow.SetImage(((TurretBuild)aux).possibleAbilities[((TurretBuild)aux).originalAbility][aux.currentLevel - 1]);

        aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
