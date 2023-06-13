using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineriaBuild : BuildingBase
{
    protected override void InternalAction()
    {
        base.InternalAction();

        buttonsFuncs.AddRange(new Pictionarys<string, System.Action>()
        {
            {"Open", Internal}
        });
    }

    void Internal()
    {

    }
}