using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableExit : LogicActive<TutorialManager>
{
    protected override void InternalActivate(params TutorialManager[] specificParam)
    {
        specificParam[0].dirigible.SetActive(true);
        specificParam[0].dialogEnable = false;
    }
}
