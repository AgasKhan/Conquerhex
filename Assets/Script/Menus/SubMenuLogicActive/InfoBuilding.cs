using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBuilding : LogicActive<BuildingBase>
{
    protected override void InternalActivate(params BuildingBase[] specificParam)
    {
        var aux = specificParam[0];
        aux.myBuildSubMenu.detailsWindow.SetTexts(aux.structureBase.nameDisplay, aux.structureBase.GetDetails().ToString());
        aux.myBuildSubMenu.detailsWindow.SetImage(aux.structureBase.image);
        aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
