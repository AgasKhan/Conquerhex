using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAction : InteractAction<(Character customer, string recipeName)>
{
    public override void Activate((Character customer, string recipeName) specificParam)
    {
        var customer = specificParam.customer;
        var recipeName = specificParam.recipeName;

        if (customer == null)
            return;

        Recipes recipe = null;

        foreach (var item in ((CraftingBuild)interactComp.container).currentRecipes)
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

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new CraftingSubMenu(this);
    }
}
