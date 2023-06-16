using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Client / Controller
public class EnterBuilding : LogicActive<BuildingBase>
{
    protected override void InternalActivate(params BuildingBase[] specificParam)
    {
        specificParam[0].EnterBuild();
        specificParam[0].myBuildSubMenu.DestroyCraftButtons();
    }
}