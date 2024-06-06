using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaryActionEvent : LogicActive<TutorialScenaryManager>
{
    public string nameOfAction = "";
    public override void Activate(TutorialScenaryManager specificParam)
    {
        specificParam.CallBetterEvent(nameOfAction);
    }
}
