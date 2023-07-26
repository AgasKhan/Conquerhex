using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToBase : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        LoadSystem.instance.Load("Base");
    }
}
