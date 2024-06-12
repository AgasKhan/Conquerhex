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
