using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMinerBuild : BuildingBase
{
    public float timeToGenerate;

    public int priorityPoints;

    public Pictionarys<ItemBase, int> generatedItems = new Pictionarys<ItemBase, int>();

    public Pictionarys<int, Pictionarys<ItemBase, int>> levelItems = new Pictionarys<int, Pictionarys<ItemBase, int>>();

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


    public void GenerateItems()
    {
        AddOrSubstractItems(generatedItems.RandomPic().nameDisplay, 1);
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        generatedItems.AddRange(levelItems[currentLevel]);
    }
}
