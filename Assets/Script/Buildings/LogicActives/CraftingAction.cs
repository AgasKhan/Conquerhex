using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAction : InteractAction<(Character customer, ItemCrafteable recipeName)>
{
    public override void Activate((Character customer, ItemCrafteable recipeName) specificParam)
    {
        var customer = specificParam.customer;
        var itemToCraft = specificParam.recipeName;

        if (customer == null || itemToCraft == null)
            return;

        if (itemToCraft.CanCraft(customer.inventory))
        {
            itemToCraft.Craft(customer.inventory, itemToCraft.nameDisplay);

            if(itemToCraft is MeleeWeaponBase)
            {
                ((CraftingSubMenu)subMenu).RefreshDetailW(itemToCraft);
            }
            else
            {
                ((CraftingBuild)interactComp.container).currentRecipes.Remove(itemToCraft);
                ((CraftingSubMenu)subMenu).Create(customer);
            }
            return;
        }
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new CraftingSubMenu(this);
    }
}
