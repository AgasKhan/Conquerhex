using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InventorySubMenu : CreateSubMenu
{
    public Character character;

    public ScrollVertComponent itemList;

    List<ButtonA> buttonsList = new List<ButtonA>();

    List<EventsCall> buttonsListActions = new List<EventsCall>();

    DetailsWindow myDetailsW;

    public void ExchangeItems(StaticEntity playerInv, StaticEntity storageInv, Item itemToMove)
    {
        itemToMove.GetAmounts(out int actual, out int max);
        playerInv.AddOrSubstractItems(itemToMove.nameDisplay, -actual);
        storageInv.AddOrSubstractItems(itemToMove.nameDisplay, actual);
    }

    public override void Create()
    {
        buttonsList.Clear();
        subMenu.ClearBody();
        base.Create();
    }

    protected override void InternalCreate()
    {
        CreateNavBar
        (
            (submenu) =>
            {
                submenu.AddNavBarButton("All", ButtonAct).AddNavBarButton("Equipment", () => { ButtonAct(ItemType.Equipment); })
                    .AddNavBarButton("Mineral", () => { ButtonAct(ItemType.Mineral); }).AddNavBarButton("Gemstone", () => { ButtonAct(ItemType.Gemstone); })
                    .AddNavBarButton("Other", () => { ButtonAct(ItemType.Other); });
            }
        );

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        foreach (var item in character.inventory)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            string details = "";

            UnityEngine.Events.UnityAction action = 
                () => 
                { 
                    ShowItemDetails(button);
                    CreateButtonsActions(item.GetItemBase().buttonsAcctions);
                };

            if (item.itemType == ItemType.Equipment && item is MeleeWeapon)
            {
                details = "Uses: " + ((MeleeWeapon)item).durability.current;

            }
            else
            {
                item.GetAmounts(out int actual, out int max);
                details = actual + " / " + max;

            }

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, details, action, item.nameDisplay));
        }

        subMenu.CreateSection(3, 6);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
    }

    public void CreateButtonsActions(Dictionary<string, System.Action<Character>> dic)
    {
        foreach (var item in buttonsListActions)
        {
            Object.Destroy(item.gameObject);
        }

        buttonsListActions.Clear();

        foreach (var item in dic)
        {
            buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set(item.Key, () => item.Value(character), ""));
        }
    }

    void ShowItemDetails(ButtonA button)
    {
        myDetailsW.SetTexts(button.myItem.nameDisplay, button.myItem.GetDetails().ToString()).SetImage(button.myItem.image);

        subMenu.RetardedOn(myDetailsW.gameObject);
    }

    void ButtonAct(ItemType type)
    {
        foreach (var item in buttonsList)
        {
            if (item.myItem.itemType != type)
                item.SetActiveGameObject(false);
            else
                item.SetActiveGameObject(true);
        }

        myDetailsW.SetActiveGameObject(false);
    }

    void ButtonAct()
    {
        foreach (var item in buttonsList)
        {
            item.SetActiveGameObject(true);
        }
    }

    void RefreshList(ItemType type)
    {
        foreach (var item in character.inventory)
        {
            if (item.itemType != type)
                continue;

            itemList.listItems.Add(item);
        }

        itemList.GenerateButtonsList();
    }

    void RefreshList()
    {
        itemList.listItems = character.inventory;
        itemList.GenerateButtonsList();
    }

    void RefreshEquipList()
    {
        foreach (var item in character.inventory)
        {
            if (item.itemType != ItemType.Equipment)
                continue;

            var aux = subMenu.componentMenu.SearchComponent<ButtonA>();
            aux.SetButtonA(item.nameDisplay, item.image, "", null, item.nameDisplay);

            itemList.GenerateButton(aux);
        }
    }
}
