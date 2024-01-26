using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();

    public List<Recipes> currentRecipes = new List<Recipes>();

    public override string rewardNextLevel
    {
        get
        {
            string aux = "";
            foreach (var item in levelRecipes[currentLevel + 1])
                aux += "\n" + item.result.Item.nameDisplay;
            
            return aux;
        }
    }

    public override void EnterBuild()
    {
        controller.EnterBuild();
    }
    public override void UpgradeLevel()
    {
        controller.UpgradeLevel();
    }
}