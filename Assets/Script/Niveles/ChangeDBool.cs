using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDBool : LogicActive<TutorialScenaryManager>
{
    protected override void InternalActivate(params TutorialScenaryManager[] specificParam)
    {
        //specificParam[0].dialogAble = !specificParam[0].dialogAble;
        specificParam[0].dialogEnable = false;
    }
}
