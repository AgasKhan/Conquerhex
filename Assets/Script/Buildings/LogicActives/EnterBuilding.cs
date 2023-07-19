using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Client / Controller
public class EnterBuilding : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        specificParam[0].myBuildSubMenu.DestroyCraftButtons();
        specificParam[0].EnterBuild();
    }
}