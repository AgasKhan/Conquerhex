using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMinerBuild : Building
{
    public float timeToGenerate;

    public int priorityPoints;

    public Pictionarys<ItemBase, int> generatedItems = new Pictionarys<ItemBase, int>();

    public Pictionarys<int, Pictionarys<ItemBase, int>> levelItems = new Pictionarys<int, Pictionarys<ItemBase, int>>();

    public override string rewardNextLevel => throw new System.NotImplementedException();


    public void GenerateItems()
    {
        //AddOrSubstractItems(generatedItems.RandomPic().nameDisplay, 1);
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        generatedItems.AddRange(levelItems[currentLevel]);
    }
}
