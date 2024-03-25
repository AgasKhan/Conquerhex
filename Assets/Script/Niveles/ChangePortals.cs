using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePortals : LogicActive<TutorialScenaryManager>
{
    public override void Activate(TutorialScenaryManager specificParam)
    {
        specificParam.firstHexagon.ladosArray = specificParam.newBorders;
        specificParam.firstHexagon.SetRenders();
        specificParam.dialogEnable = false;
    }
}
