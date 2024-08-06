using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAction : InteractAction<(Character customer, ItemCrafteable recipeName)>
{
    AudioEntityComponent audioComponent;

    public override void Activate((Character customer, ItemCrafteable recipeName) specificParam)
    {
        var customer = specificParam.customer;
        var itemToCraft = specificParam.recipeName;

        if (customer == null || itemToCraft == null)
            return;

        if (itemToCraft.CanCraft(customer.inventory))
        {
            itemToCraft.Craft(customer.inventory, itemToCraft.nameDisplay);
            //audioComponent.Play("CraftAudio");

            if (itemToCraft is MeleeWeaponBase)
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
        else
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "No tienes suficientes materiales")
                        .AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
        }
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new CraftingSubMenu(this);
        audioComponent = GetComponent<AudioEntityComponent>();
    }
}
