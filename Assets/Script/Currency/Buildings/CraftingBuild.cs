using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();
    public CraftingSubMenu createSubMenu;

    public List<Recipes> recipes = new List<Recipes>();
    //---------------------------------
    public MenuManager refMenu;
    //---------------------------------
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
        //---------------------------------
        refMenu.eventListVoid.Add("Try3", Try3);
        //---------------------------------
    }
    //---------------------------------
    void Try3 (GameObject g)
    {
        myBuildSubMenu.ClearSubMenu();
        myBuildSubMenu.Create();
    }
    //---------------------------------
    public void ClearCustomerInventory()
    {
        character.inventory.Clear();
    }
    public override void EnterBuild()
    {
        createSubMenu.Create();
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        for (int i = 0; i <levelRecipes[currentLevel].Count; i++)
        {
            recipes.Add(levelRecipes[currentLevel][i]);
        }

        if(currentLevel == 1)
        {
            //buttonsFuncs.Add("Craft", ShowCraftingW);
        }

        myBuildSubMenu.ClearSubMenu();
        myBuildSubMenu.Create();
    }
}