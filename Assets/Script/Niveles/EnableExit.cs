using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableExit : LogicActive<TutorialScenaryManager>
{
    protected override void InternalActivate(params TutorialScenaryManager[] specificParam)
    {
        specificParam[0].dirigible.SetActive(true);
        specificParam[0].dialogEnable = false;
    }
}
