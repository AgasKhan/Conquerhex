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

    List<Item> ItemBuffer = new List<Item>();

    List<ButtonA> buttonsList = new List<ButtonA>();

    PopUp myPopUp;

    public List<Item> CompareType(GameObject g)
    {
        ItemBuffer = character.inventory;

        for (int i = 0; i < ItemBuffer.Count; i++)
        {
            if (ItemBuffer[i].itemType.ToString() != g.name)
            {
                ItemBuffer.RemoveAt(i);
            }
        }

        return ItemBuffer;
    }

    public List<Item> RemoveItem(GameObject g)
    {
        character.AddOrSubstractItems(g.name, 1);
        ItemBuffer = character.inventory;

        return ItemBuffer;
    }
    public List<Item> RemoveItem(string itemName, int amount)
    {
        character.AddOrSubstractItems(itemName, -amount);
        ItemBuffer = character.inventory;

        return ItemBuffer;
    }

    public void ExchangeItems(StaticEntity playerInv, StaticEntity storageInv, Item itemToMove)
    {
        itemToMove.GetAmounts(out int actual, out int max);
        playerInv.AddOrSubstractItems(itemToMove.nameDisplay, -actual);
        storageInv.AddOrSubstractItems(itemToMove.nameDisplay, actual);
    }

    protected override void InternalCreate()
    {
        subMenu.ClearBody();
        
        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        foreach (var item in character.inventory)
        {
            ButtonA button;
            
            if(item.itemType == ItemType.Equipment)
            {
                var aux = item as MeleeWeapon;
                var aux2 = item as RangeWeapon;

                if(aux != null || aux2 != null)
                {
                    button = subMenu.AddComponent<ButtonA>();
                    buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, "Uses: " + aux.durability.current, () => { ShowEquipDetails(button); }, item.nameDisplay));
                }
            }
            else
            {
                button = subMenu.AddComponent<ButtonA>();
                item.GetAmounts(out int actual, out int max);
                buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, actual + " / " + max, () => { ShowItemDetails(button); }, item.nameDisplay));
            }
        }

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        myPopUp = subMenu.AddComponent<PopUp>().SetActiveGameObject(true);

        //myPopUp = MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true);


        //myDetailsW = subMenu.AddComponent<DetailsWindow>().SetTexts("","");


        CreateNavBar
        (
            (submenu) =>
            {
            submenu.AddNavBarButton("All", ButtonAct).AddNavBarButton("Equipment",()=>{ ButtonAct(ItemType.Equipment);})
                .AddNavBarButton("Mineral",()=>{ButtonAct(ItemType.Mineral);}).AddNavBarButton("Gemstone",()=>{ButtonAct(ItemType.Gemstone);})
                .AddNavBarButton("Other",()=>{ButtonAct(ItemType.Other);});
            }
        );
        
    }
    void ShowItemDetails(ButtonA button)
    {
        myPopUp.SetWindow(button.myItem.nameDisplay, button.myItem.GetDetails().ToString(), button.myItem.image);
    }
    void ShowEquipDetails(ButtonA button)
    {
        myPopUp.SetWindow(button.myItem.nameDisplay, button.myItem.GetDetails().ToString(), button.myItem.image);
        myPopUp.AddButton("Equip", "Equip");

        //subMenu.AddComponent<EventsCall>().Clone("Equip", null, "Equip", subMenu.lastSectionLayoutGroup.transform);
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
