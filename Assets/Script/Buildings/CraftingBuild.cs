using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building, ISaveObject
{
    public Pictionarys<int, List<ItemCrafteable>> levelRecipes = new Pictionarys<int, List<ItemCrafteable>>();

    public virtual List<ItemCrafteable> currentRecipes => data.currentRecipes;

    SvData data;

    public override string rewardNextLevel
    {
        get
        {
            string aux = "";
            foreach (var item in levelRecipes[currentLevel + 1])
                aux += "\n" + item.nameDisplay;
            
            return aux;
        }
    }

    public override void UpgradeLevel()
    {
        controller.UpgradeLevel();
    }
    /*
    public string Save()
    {
        SvData data = new SvData(currentRecipes, currentLevel);

        return JsonUtility.ToJson(data);
    }

    public void Load(string str)
    {
        SvData data = JsonUtility.FromJson<SvData>(str);

        currentLevel = data.currentLevel;
    }
    */
    [System.Serializable]
    public class SvData
    {
        public List<ItemCrafteable> currentRecipes;
        public int currentLevel;

        public SvData(List<ItemCrafteable> list, int level)
        {
            currentRecipes = new List<ItemCrafteable>(list);
            currentLevel = level;
        }
    }
}