using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePortals : LogicActive<TutorialManager>
{
    protected override void InternalActivate(params TutorialManager[] specificParam)
    {
        specificParam[0].firstHexagon.ladosArray = specificParam[0].newBorders;
        specificParam[0].firstHexagon.SetRenders();
    }
}
