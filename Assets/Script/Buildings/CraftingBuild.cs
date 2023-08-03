using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();

    [HideInInspector]
    public CraftingSubMenu createSubMenu;

    public List<Recipes> currentRecipes = new List<Recipes>();

    public override string rewardNextLevel
    {
        get
        {
            string aux = "";
            foreach (var item in levelRecipes[currentLevel + 1])
            {
                aux += "\n" + item.result.Item.nameDisplay;
            }
            return aux;
        }
    }

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        createSubMenu = new CraftingSubMenu(this);

        if (SaveWithJSON.BD.ContainsKey(flyweight.nameDisplay + "Level"))
        {
            interact.Add("Craftear", GetComponent<EnterBuilding>());
            currentRecipes = SaveWithJSON.LoadFromPictionary<List<Recipes>>(flyweight.nameDisplay + "Recipes");
        }
        else
        {
            interact.Remove("Craftear");
            currentRecipes.Clear();
        }
    }

    public override void EnterBuild()
    {
        createSubMenu.Create();
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        for (int i = 0; i < levelRecipes[currentLevel].Count; i++)
        {
            currentRecipes.Add(levelRecipes[currentLevel][i]);
        }

        if (currentLevel == 1)
        {
            interact.Add("Craftear", GetComponent<EnterBuilding>());
        }

        SaveWithJSON.SaveInPictionary(flyweight.nameDisplay + "Recipes", currentRecipes);
        myBuildSubMenu.Create();
    }
}