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

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        subMenu.AddNavBarButton("All", ButtonAct).AddNavBarButton("Equipment", () => { ButtonAct(ItemType.Equipment.ToString()); })
                    .AddNavBarButton("Mineral", () => { ButtonAct(ItemType.Mineral.ToString()); }).AddNavBarButton("Gemstone", () => { ButtonAct(ItemType.Gemstone.ToString()); })
                    .AddNavBarButton("Other", () => { ButtonAct(ItemType.Other.ToString()); });

        subMenu.CreateTitle("Inventory");

        CreateBody();
    }

    void CreateBody()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateButtons();

        subMenu.CreateSection(3, 6);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
    }

    public void CreateButtons()
    {
        buttonsList.Clear();

        for (int i = 0; i < character.inventory.Count; i++)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            var item = character.inventory[i];

            var index = i;

            UnityEngine.Events.UnityAction action =
               () =>
               {
                   ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                   DestroyButtonsActions();
                   CreateButtonsActions(index, item.GetItemBase().buttonsAcctions);
               };

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action).SetType(item.itemType.ToString()));
        }
    }

    void DestroyButtonsActions()
    {

        foreach (var item in buttonsListActions)
        {
            if (item != null)
                Object.Destroy(item.gameObject);
        }

        buttonsListActions.Clear();
    }

    void CreateButtonsActions(int myItem, Dictionary<string, System.Action<Character, int>> dic)
    {

        foreach (var item in dic)
        {
            buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set(item.Key, 
                () => {
                    item.Value(character, myItem);
                    CreateBody();
                }, ""));

            buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(300, 75);

            //subMenu.AddComponent<EventsCall>().Set(item.Key, () => item.Value(character), "");
        }
    }


    void ShowItemDetails(string nameDisplay, string details, Sprite Image)
    {       
        myDetailsW.SetTexts(nameDisplay,details).SetImage(Image);
        myDetailsW.SetActiveGameObject(true);
    }

    void ButtonAct(string type)
    {
        foreach (var item in buttonsList)
        {
            if (item.type != type && type != "")
                item.SetActiveGameObject(false);
            else
                item.SetActiveGameObject(true);
        }

        myDetailsW.SetActiveGameObject(false);
        DestroyButtonsActions();
    }

    void ButtonAct()
    {
        ButtonAct("");
    }

    string SetTextforItem(Item item)
    {
        string details = "";

        if (item.itemType == ItemType.Equipment && item is MeleeWeapon)
        {
            details = "Uses: " + ((MeleeWeapon)item).actual;

        }
        else
        {
            item.GetAmounts(out int actual, out int max);
            details = actual + " / " + max;

        }

        return details;
    }

}
