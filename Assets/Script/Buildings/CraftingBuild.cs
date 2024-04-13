using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building, ISaveObject
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

    public string Save()
    {
        CraftBuildData data = new CraftBuildData(currentRecipes, currentLevel);

        return JsonUtility.ToJson(data);
    }

    public void Load(string str)
    {
        CraftBuildData data = JsonUtility.FromJson<CraftBuildData>(str);

        currentRecipes = data.currentRecipes.value;
        currentLevel = data.currentLevel;
    }
}

[System.Serializable]
public class CraftBuildData
{
    public AuxClass<List<MeleeWeaponBase>> currentRecipes;
    public int currentLevel;

    public CraftBuildData(List<MeleeWeaponBase> list, int level)
    {
        currentRecipes = new AuxClass<List<MeleeWeaponBase>>(list);
        currentLevel = level;
    }
}