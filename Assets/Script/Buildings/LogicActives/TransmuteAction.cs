using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmuteAction : InteractAction<(Character character, MeleeWeapon item)>
{
    public override void Activate((Character character, MeleeWeapon item) genericParams)
    {
        Character character = genericParams.character;
        MeleeWeapon item = genericParams.item;

        //character.inventory.InternalRemoveItem(item);

        foreach (var ingredient in (item.GetItemBase() as MeleeWeaponBase).recipe.materials)
        {
            character.inventory.AddItem(ingredient.Item, ingredient.Amount);
        }
    }
    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new GenericSubMenu(_interactComp);
        var menu = (GenericSubMenu)subMenu;

        System.Action<SubMenus> menuAction =
        (internalSubMenu) =>
        {
            internalSubMenu.CreateSection(0, 2);
            internalSubMenu.CreateChildrenSection<ScrollRect>();

            List<MeleeWeapon> crafteableItems = new List<MeleeWeapon>();

            foreach (var item in menu.myCharacter.inventory)
            {
                if (item is MeleeWeapon)
                    crafteableItems.Add(item as MeleeWeapon);
            }

            foreach (var item in crafteableItems)
            {
                internalSubMenu.AddComponent<ButtonA>().SetButtonA(item.nameDisplay, item.image, menu.SetTextforItem(item), 
                    () => 
                    { 
                        menu.DestroyLastButtons();
                        menu.detailsWindow.SetTexts(item.nameDisplay, item.GetDetails().ToString("\n") + (item.GetItemBase() as MeleeWeaponBase).recipe.GetIngredientsStr()).SetImage(item.image);
                        menu.CreateButton("Transmute", () => { menu.detailsWindow.SetActive(true); Activate((menu.myCharacter, item)); internalSubMenu.gameObject.SetActive(false); });

                    }).rectTransform.sizeDelta = new Vector2(300, 150);
            }

            internalSubMenu.CreateSection(2, 6);
            internalSubMenu.CreateChildrenSection<ScrollRect>();
            menu.detailsWindow = internalSubMenu.AddComponent<DetailsWindow>().SetActive(false);

            if (crafteableItems.Count < 1)
            {
                menu.detailsWindow.SetTexts("You do not have items to transmute", "");
            }

            internalSubMenu.CreateTitle(menu.interactComponent.container.flyweight.nameDisplay);

        };

        menu.SetCreateAct(menuAction);
    }

}
