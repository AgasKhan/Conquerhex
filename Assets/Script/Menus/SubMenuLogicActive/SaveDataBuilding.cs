using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataBuilding : LogicActive<SaveBuild>
{
    protected override void InternalActivate(params SaveBuild[] specificParam)
    {
        specificParam[0].SaveBaseData();
    }
}
