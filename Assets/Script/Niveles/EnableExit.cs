using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableExit : LogicActive<TutorialScenaryManager>
{
    public override void Activate(TutorialScenaryManager specificParam)
    {
        specificParam.goal.SetActive(true);
        specificParam.dialogEnable = false;
        SaveWithJSON.SaveInPictionary("FirstTime", true);
    }
}
