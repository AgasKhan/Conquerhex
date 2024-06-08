using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAction : InteractAction<(Character customer, ItemCrafteable recipeName)>
{
    public override void Activate((Character customer, ItemCrafteable recipeName) specificParam)
    {
        var customer = specificParam.customer;
        var itemToCraft = specificParam.recipeName;

        if (customer == null)
            return;

        Recipes recipe = null;

        foreach (var item in ((CraftingBuild)interactComp.container).currentRecipes)
        {
            if (itemToCraft == item)
            {
                recipe = item.recipe;
                break;
            }
        }

        if (recipe == null)
        {
            Debug.Log("No se encontro la receta: " + itemToCraft);
            return;
        }

        if (recipe.CanCraft(customer.inventory))
        {
            recipe.Craft(customer.inventory, itemToCraft.nameDisplay);
            ((CraftingSubMenu)subMenu).RefreshDetailW(itemToCraft);
            return;
        }
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new CraftingSubMenu(this);
    }
}
