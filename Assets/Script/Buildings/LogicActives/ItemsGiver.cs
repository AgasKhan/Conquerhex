using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGiver : LogicActive<Character>
{
    public List<Ingredient> items = new List<Ingredient>();

    public List<Item> inventory = new List<Item>();

    public override void Activate(Character genericParams)
    {
        foreach (var item in items)
        {
            inventory.Add(item.Item.Create());
            inventory[inventory.Count - 1].Init(genericParams.inventory);
        }
    }
}
