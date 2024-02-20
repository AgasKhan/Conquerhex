using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuildController : BuildingsController
{
    [HideInInspector]
    public CraftingSubMenu createSubMenu;

    [HideInInspector]
    public CraftingBuild craftBuild;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        craftBuild = GetComponent<CraftingBuild>();

        createSubMenu = new CraftingSubMenu(craftBuild);

        /*

        if (SaveWithJSON.BD.ContainsKey(craftBuild.flyweight.nameDisplay + "Level"))
        {
            craftBuild.interact.Add("Craftear", GetComponent<EnterBuilding>());
            craftBuild.currentRecipes = SaveWithJSON.LoadFromPictionary<List<Recipes>>(craftBuild.flyweight.nameDisplay + "Recipes");
        }
        else
        {
            craftBuild.interact.Remove("Craftear");
            craftBuild.currentRecipes.Clear();
        }

        */
    }


    public override void EnterBuild()
    {
        createSubMenu.Create();
    }

    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        for (int i = 0; i < craftBuild.levelRecipes[craftBuild.currentLevel].Count; i++)
        {
            craftBuild.currentRecipes.Add(craftBuild.levelRecipes[craftBuild.currentLevel][i]);
        }

        if (craftBuild.currentLevel == 1)
        {
            craftBuild.interact.Add("Craftear", GetComponent<EnterBuilding>());
        }

        SaveWithJSON.SaveInPictionary(craftBuild.flyweight.nameDisplay + "Recipes", craftBuild.currentRecipes);
        craftBuild.myBuildSubMenu.Create();
    }
}
