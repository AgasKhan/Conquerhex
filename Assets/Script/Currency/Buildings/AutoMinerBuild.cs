using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMinerBuild : BuildingBase
{
    public float timeToGenerate;

    public int priorityPoints;

    public Pictionarys<ItemBase, int> generatedItems = new Pictionarys<ItemBase, int>();

    public Pictionarys<int, Pictionarys<ItemBase, int>> levelItems = new Pictionarys<int, Pictionarys<ItemBase, int>>();

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
