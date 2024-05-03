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

        subMenu.Create((subMenu as GenericSubMenu).myCharacter);
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
            
            foreach (var item in inventoryFrom)
            {
                if (item is Ability && !((Ability)item).visible)
                    continue;

                ButtonA button = internalSubMenu.AddComponent<ButtonA>();

                menu.buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, menu.SetTextforItem(item), () =>
                {
                    menu.ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                    menu.DestroyLastButtons();
                    menu.CreateButton("Mover item del " + inventoryFrom.container.flyweight.nameDisplay + " al " + inventoryTo.container.name, () => Activate((inventoryTo, item))).rectTransform.sizeDelta = new Vector2(400, 85);
                }
                ));
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
