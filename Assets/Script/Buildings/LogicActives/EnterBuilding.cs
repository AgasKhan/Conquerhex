using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Client / Controller
public class EnterBuilding : InteractAction<Building>
{
    public override void Activate(Building specificParam)
    {
        specificParam.myBuildSubMenu.Create();

        //specificParam.myBuildSubMenu.DestroyCraftButtons();
        //specificParam.EnterBuild();
    }
}