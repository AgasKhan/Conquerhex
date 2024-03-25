using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBuilding : InteractAction<Building>
{
    public override void Activate(Building specificParam)
    {
        var aux = specificParam;
        //aux.myBuildSubMenu.detailsWindow.SetTexts("", aux.flyweight.GetDetails()["Description"]).SetImage(aux.flyweight.image);
        //aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
