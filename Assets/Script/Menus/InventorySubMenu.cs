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

        CreateButtons();

        subMenu.CreateSection(3, 6);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
    }

    public void CreateButtons()
    {
        foreach (var item in buttonsList)
        {
            Object.Destroy(item.gameObject);
        }

        buttonsList.Clear();

        foreach (var item in character.inventory)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            UnityEngine.Events.UnityAction action =
                () =>
                {
                    ShowItemDetails(button);
                    CreateButtonsActions(item.GetItemBase().buttonsAcctions);
                };

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action, item.nameDisplay));
        }
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

            buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(300, 75);

            //subMenu.AddComponent<EventsCall>().Set(item.Key, () => item.Value(character), "");
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

    void RefreshList()
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            if(buttonsList[i].myItem == character.inventory[i])
            {
                buttonsList[i].textButton.text = SetTextforItem(character.inventory[i]);
            }
        }
    }

    string SetTextforItem(Item item)
    {
        string details = "";

        if (item.itemType == ItemType.Equipment && item is MeleeWeapon)
        {
            details = "Uses: " + ((MeleeWeapon)item).durability.current;

        }
        else
        {
            item.GetAmounts(out int actual, out int max);
            details = actual + " / " + max;

        }

        return details;
    }

}
