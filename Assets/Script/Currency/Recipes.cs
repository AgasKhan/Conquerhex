using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Recipe", fileName = "Recipe")]
public class Recipes : ShowDetails
{
    public List<Ingredient> materials;

    public List<Ingredient> results;

    
    public bool CanCraft(IItemContainer container)
    {
        foreach (var ingredient in materials)
        {
            if (container.ItemCount(ingredient.Item.Create()) < ingredient.Amount)
                return false;
        }
        return true;
    }

    public void Craft(IItemContainer container)
    {
        if(CanCraft(container))
        {
            foreach (var ingredient in materials)
            {
                container.RemoveItems(ingredient.Item.Create(), ingredient.Amount);
            }

            foreach (var ingredient in results)
            {
                container.AddItems(ingredient.Item.Create(), ingredient.Amount);
            }
        }
    }
    
}

[Serializable]
public struct Ingredient
{
    public ItemBase Item;

    [Range(1, 50)]
    public int Amount;
}