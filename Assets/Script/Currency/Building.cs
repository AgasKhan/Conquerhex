using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Building : StaticEntity
{
    public Pictionarys<string, Action> buttonsFuncs = new Pictionarys<string, Action>();


    [SerializeField]
    StructureBase structureBase;
    protected override Damage[] vulnerabilities => structureBase.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyOnEnables += MyOnEnable;
    }

    private void MyOnEnable()
    {

        buttonsFuncs.AddRange(new Pictionarys<string, Action>()
        {
            // Static Buttons
            {"Open", Example}
        });

    }

    void Example()
    {

    }
}
