using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAction : InteractAction<(Character customer, string recipeName)>
{
    public Entity entity;
    public override void Activate((Character customer, string recipeName) specificParam)
    {
        var customer = specificParam.customer;
        var recipeName = specificParam.recipeName;

        if (customer == null)
            return;

        Recipes recipe = null;

        foreach (var item in ((CraftingBuild)entity).currentRecipes)
        {
            if (recipeName == item.name)
            {
                recipe = item;
                break;
            }
        }

        if (recipe == null)
        {
            Debug.Log("No se encontro la receta: " + recipeName);
            return;
        }

        if (recipe.CanCraft(customer.inventory))
        {
            recipe.Craft(customer.inventory);
            ((CraftingSubMenu)subMenu).RefreshDetailW(recipe);
            return;
        }
    }

    public CraftingAction(Entity _entity)
    {
        entity = _entity;
        subMenu = new CraftingSubMenu(this);
    }
}
