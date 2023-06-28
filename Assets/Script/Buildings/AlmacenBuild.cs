using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmacenBuild : Building
{
    public override string rewardNextLevel => throw new System.NotImplementedException();

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        
    }

    void Internal()
    {

    }
}
