using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollVertComponent : MonoBehaviour
{
    [SerializeField]
    public List<Item> listItems = new List<Item>();

    [SerializeField]
    public ButtonsAFactory buttons;

    public void AddItemToList(Item item)
    {
        listItems.Add(item);
    }
    public void RemoveItemToList(Item item)
    {
        listItems.Remove(item);
    }

    public void GenerateButtonsList(List<Item> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if(item.itemType == ItemType.Equipment)
            {
                var aux = item as MeleeWeapon;
                buttons.CreateAButtons(item.nameDisplay, "ModifierDW", item.image, aux.durability.current.ToString(), null);
            }
            else
            {
                item.GetAmounts(out int actual, out int max);
                buttons.CreateAButtons(item.nameDisplay, "ModifierDW", item.image, actual + " / " + max, null);
            }
            
        }
    }

}
