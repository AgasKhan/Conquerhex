using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBuild : Building
{
    public Pictionarys<string, List<Item>> allInventories = new Pictionarys<string, List<Item>>();
    public override string rewardNextLevel => throw new System.NotImplementedException();

    public void SaveBaseData()
    {
        SaveWithJSON.SaveGame();
    }

}
