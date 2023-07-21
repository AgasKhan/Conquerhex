using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDBool : LogicActive<TutorialManager>
{
    protected override void InternalActivate(params TutorialManager[] specificParam)
    {
        //specificParam[0].dialogAble = !specificParam[0].dialogAble;
        specificParam[0].dialogEnable = false;
    }
}
