using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBuilding : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        var aux = specificParam[0];
        aux.myBuildSubMenu.detailsWindow.SetTexts("", aux.structureBase.GetDetails()["Description"]).SetImage(aux.structureBase.image);
        aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
