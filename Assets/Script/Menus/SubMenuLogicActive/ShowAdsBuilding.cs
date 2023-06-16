using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAdsBuilding : LogicActive<AdsBuilding>
{
    protected override void InternalActivate(params AdsBuilding[] specificParam)
    {
        specificParam[0].ShowAd();
    }
}
