using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BuildingBase : StaticEntity
{
    public Pictionarys<string, Action> buttonsFuncs = new Pictionarys<string, Action>();
    public float workTime;

    protected Character character;
    public int maxLevel;
    public int currentLevel = 0;

    [SerializeField]
    StructureBase structureBase;
    protected override Damage[] vulnerabilities => structureBase.vulnerabilities;
    

    protected override void Config()
    {
        base.Config();

        MyOnEnables += InternalAction;
    }

    protected virtual void InternalAction()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        character = other.GetComponent<Character>();

    }

    public virtual void UpgradeLevel()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;
    }

}