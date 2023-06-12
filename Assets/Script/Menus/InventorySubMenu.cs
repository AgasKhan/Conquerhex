using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySubMenu : MonoBehaviour
{
    public SubMenus subMenu;
    
    public Character character;

    public ScrollVertComponent itemList;

    List<Item> ItemBuffer = new List<Item>();

    public void BodyCreate()
    {
        subMenu.CreateSection(0, 3);
            itemList = subMenu.AddComponent<ScrollVertComponent>();
        //aux.listItems.AddRange(player.inventory);
            itemList.GenerateButtonsList(character.inventory);
        /*
        CreateSection(3, 6);
            AddComponent<DetailsWindow>().SetTexts("", "").SetImage();
        */
    }

    public void RefreshInventory()
    {
        //itemList.
    }

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

}
