using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : Building, ISaveObject
{
    public Pictionarys<int, List<MeleeWeaponBase>> levelRecipes = new Pictionarys<int, List<MeleeWeaponBase>>();

    public List<MeleeWeaponBase> currentRecipes => data.currentRecipes;

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

    [System.Serializable]
    public class SvData
    {
        public List<MeleeWeaponBase> currentRecipes;
        public int currentLevel;

        public SvData(List<MeleeWeaponBase> list, int level)
        {
            currentRecipes = new List<MeleeWeaponBase>(list);
            currentLevel = level;
        }
    }
}