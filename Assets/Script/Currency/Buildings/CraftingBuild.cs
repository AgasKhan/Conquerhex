using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();

    [HideInInspector]
    public CraftingSubMenu createSubMenu;

    public List<Recipes> currentRecipes = new List<Recipes>();
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
        createSubMenu = new CraftingSubMenu(this);
        //---------------------------------
        refMenu.eventListVoid.Add("Try3", Try3);
        //---------------------------------
        if (SaveWithJSON.BD.ContainsKey(structureBase.nameDisplay + "Level"))
        {
            interact.Add("Craftear", GetComponent<EnterBuilding>());
            currentRecipes = SaveWithJSON.LoadFromPictionary<List<Recipes>>(structureBase.nameDisplay + "Recipes");
        }
        else
        {
            interact.Remove("Craftear");
            currentRecipes.Clear();
        }
    }
    //---------------------------------
    void Try3 (GameObject g)
    {
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

        for (int i = 0; i < levelRecipes[currentLevel].Count; i++)
        {
            currentRecipes.Add(levelRecipes[currentLevel][i]);
        }

        if (currentLevel == 1)
        {
            interact.Add("Craftear", GetComponent<EnterBuilding>());
        }

        SaveWithJSON.SaveInPictionary(structureBase.nameDisplay + "Recipes", currentRecipes);
        myBuildSubMenu.Create();
    }
}