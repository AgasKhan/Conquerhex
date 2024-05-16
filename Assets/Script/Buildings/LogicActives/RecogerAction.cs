using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecogerAction : InteractAction<(InventoryEntityComponent inventTo, Item item)>
{
    public bool toCharacter = true;
    public override void Activate((InventoryEntityComponent inventTo, Item item) genericParam)
    {
        genericParam.inventTo.AddItem(genericParam.item);
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new GenericSubMenu(_interactComp);
        GenericSubMenu menu = subMenu as GenericSubMenu;

        System.Action<SubMenus> menuAction =
        (internalSubMenu) =>
        {
            internalSubMenu.CreateSection(0, 3);
            internalSubMenu.CreateChildrenSection<ScrollRect>();

            InventoryEntityComponent inventoryFrom = toCharacter ? _interactComp.container.GetInContainer<InventoryEntityComponent>() : menu.myCharacter.GetInContainer<InventoryEntityComponent>();
            InventoryEntityComponent inventoryTo = toCharacter ? menu.myCharacter.GetInContainer<InventoryEntityComponent>() : _interactComp.container.GetInContainer<InventoryEntityComponent>();

            List<Item> allItems = new List<Item>();

            foreach (var item in inventoryFrom)
            {
                if ((item is Ability && !((Ability)item).visible) || (item is ItemEquipable && !((ItemEquipable)item).isRemovable))
                    continue;

                allItems.Add(item);
                ButtonA button = internalSubMenu.AddComponent<ButtonA>();

                menu.buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, menu.SetTextforItem(item), () =>
                {
                    menu.ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                    menu.DestroyLastButtons();
                    menu.CreateButton("Mover item del " + inventoryFrom.container.flyweight.nameDisplay + " al " + inventoryTo.container.flyweight.nameDisplay,
                        () => 
                        {
                            Activate((inventoryTo, item));
                            subMenu.Create(menu.myCharacter);
                        }).rectTransform.sizeDelta = new Vector2(400, 85);
                }
                ));
            }

            if(allItems.Count > 0)
            {
                var button = internalSubMenu.AddComponent<ButtonA>();
                button.SetButtonA("Mover todos los items al " + inventoryTo.container.name, null,"",()=>
                {
                    menu.DestroyLastButtons();
                    foreach (var itemInChest in allItems)
                    {
                        Activate((inventoryTo, itemInChest));
                    }
                    subMenu.Create(menu.myCharacter);
                }).rectTransform.sizeDelta = new Vector2(800, 200);
            }

            internalSubMenu.CreateSection(3, 6);
            internalSubMenu.CreateChildrenSection<ScrollRect>();
            menu.detailsWindow = internalSubMenu.AddComponent<DetailsWindow>().SetTexts("", "").SetImage(null);

            if (inventoryFrom.Count <= 0)
                menu.ShowItemDetails(toCharacter? "Cofre vacío" : "Inventario vacío", toCharacter ? "Nada que ver aquí": "No tienes ningun item", null);
            

            internalSubMenu.CreateTitle("Cofre");
        };

        menu.SetCreateAct(menuAction);
    }
}
