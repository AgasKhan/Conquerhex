using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipesImprovement : SingletonMono<RecipesImprovement>
{
    public Pictionarys<ItemBase, Improvement> improvements = new Pictionarys<ItemBase, Improvement>();

    public List<MeleeWeapon> weaponToImprove;

    public MeleeWeapon ImproveWeapon(MeleeWeapon weapon, ItemBase material)
    {
        if (!IsAbleToImprove(weapon, material))
            return default;
        
        for (int i = 0; i < improvements[material].dmgImprovements.Length; i++)
        {
            if (weapon.damages[i].typeInstance == improvements[material].dmgImprovements[i].typeInstance)
                weapon.damages[i].amount *= improvements[material].dmgImprovements[i].amount;
        }

        weapon.durability.Set(weapon.itemBase.durability + improvements[material].durImprovement);

        weapon.itemBase.durability += improvements[material].durImprovement;

        //weapon.Init();
        Debug.LogWarning("Falta implementar el init nuevo");

        return weapon;
    }

    public bool IsAbleToImprove(MeleeWeapon weapon, ItemBase material)
    {
        return improvements.ContainsKey(material) && weaponToImprove.Contains(weapon);
    }
}

public struct Improvement
{
    public Damage[] dmgImprovements;
    public float durImprovement;
    public Color color;
}