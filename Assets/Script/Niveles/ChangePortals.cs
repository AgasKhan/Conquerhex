using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePortals : LogicActive<TutorialScenaryManager>
{
    protected override void InternalActivate(params TutorialScenaryManager[] specificParam)
    {
        specificParam[0].firstHexagon.ladosArray = specificParam[0].newBorders;
        specificParam[0].firstHexagon.SetRenders();
        specificParam[0].dialogEnable = false;
    }
}
