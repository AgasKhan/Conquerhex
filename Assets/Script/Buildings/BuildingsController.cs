using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsController : MyScripts
{
    protected virtual Building myBuilding { get; set; }

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        myBuilding = GetComponent<Building>();

    }
    

    public virtual void EnterBuild()
    {

    }
    public virtual void UpgradeLevel()
    {
        myBuilding.currentLevel++;
        if (myBuilding.currentLevel > myBuilding.maxLevel)
            myBuilding.currentLevel = myBuilding.maxLevel;

        SaveWithJSON.SaveInPictionary(myBuilding.flyweight.nameDisplay + "Level", myBuilding.currentLevel);
    }

}
