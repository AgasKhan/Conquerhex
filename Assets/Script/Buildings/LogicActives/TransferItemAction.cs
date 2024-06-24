using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransferItemAction : InteractAction<(InventoryEntityComponent inventTo, Item item)>
{
    bool toCharacter = true;
    ListNavBarModule myListNavBar;

    public override void Activate((InventoryEntityComponent inventTo, Item item) genericParam)
    {
        genericParam.inventTo.AddItem(genericParam.item);
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        toCharacter = true;
        subMenu = new GenericSubMenu(_interactComp);
        GenericSubMenu menu = subMenu as GenericSubMenu;

        InventoryEntityComponent inventoryFrom = null;
        InventoryEntityComponent inventoryTo = null;

        System.Action createListNavBar = null;

        System.Action clearSubMenu = null;

        System.Action<SubMenus> menuAction =
        (internalSubMenu) =>
        {
            internalSubMenu.CreateSection(0, 3);
            myListNavBar = internalSubMenu.AddComponent<ListNavBarModule>();
            createListNavBar.Invoke();

            internalSubMenu.CreateSection(3, 6);
            internalSubMenu.CreateChildrenSection<ScrollRect>();
            menu.detailsWindow = internalSubMenu.AddComponent<DetailsWindow>().SetTexts("", "").SetImage(null);

            if (inventoryFrom.Count <= 0)
                menu.ShowItemDetails(toCharacter ? "Cofre vacío" : "Inventario vacío", toCharacter ? "Nada que ver aquí" : "No tienes ningun item", null);

            internalSubMenu.CreateTitle("Transferir objetos");
            
            myListNavBar.SetLeftAuxButton(inventoryFrom.container.name, () =>
            {
                toCharacter = true;
                clearSubMenu.Invoke();
            }, "");

            myListNavBar.SetRightAuxButton(inventoryTo.container.name, () =>
            {
                toCharacter = false;
                clearSubMenu.Invoke();
            }, "");
        };

        createListNavBar = () =>
        {
            inventoryFrom = toCharacter ? _interactComp.container.GetInContainer<InventoryEntityComponent>() : menu.myCharacter.GetInContainer<InventoryEntityComponent>();
            inventoryTo = toCharacter ? menu.myCharacter.GetInContainer<InventoryEntityComponent>() : _interactComp.container.GetInContainer<InventoryEntityComponent>();

            List<Item> allItems = new List<Item>();
            foreach (var item in inventoryFrom)
            {
                if ((item is Ability && !((Ability)item).visible) || (item is ItemEquipable && !((ItemEquipable)item).isRemovable))
                    continue;

                allItems.Add(item);

                menu.buttonsList.Add(myListNavBar.AddButtonHor(item.nameDisplay, item.image, item.GetItemTags(), () =>
                {
                    menu.ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                    //menu.DestroyLastButtons();
                }).SetAuxButton("Mover",
                () =>
                {
                    Activate((inventoryTo, item));
                    clearSubMenu.Invoke();
                },""));
            }

            if (allItems.Count > 0)
            {
                myListNavBar.AddButtonHor(("Mover todos los items al " + inventoryTo.container.name).RichText("color", "#edd15f"), null, default, () =>
                {
                    menu.DestroyLastButtons();
                    foreach (var itemInChest in allItems)
                    {
                        Activate((inventoryTo, itemInChest));
                    }
                    clearSubMenu.Invoke();
                });
            }

            myListNavBar.SetTitle("Del " + inventoryFrom.container.name + " al " + inventoryTo.container.name);

        };

        clearSubMenu = () =>
        {
            myListNavBar.ClearButtonsHor();
            menu.detailsWindow.Clear();
            menu.DestroyLastButtons();
            createListNavBar.Invoke();

            if (inventoryFrom.Count <= 0)
                menu.ShowItemDetails(toCharacter ? "Cofre vacío" : "Inventario vacío", toCharacter ? "Nada que ver aquí" : "No tienes ningun item", null);
        };

        menu.SetCreateAct(menuAction);
    }
}
