using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building
{
    public Pictionarys<int, List<MeleeWeaponBase>> levelRecipes = new Pictionarys<int, List<MeleeWeaponBase>>();

    public List<MeleeWeaponBase> currentRecipes = new List<MeleeWeaponBase>();

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

}