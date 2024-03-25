using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDBool : LogicActive<TutorialScenaryManager>
{
    public override void Activate(TutorialScenaryManager specificParam)
    {
        //specificParam[0].dialogAble = !specificParam[0].dialogAble;
        specificParam.dialogEnable = false;
    }
}
