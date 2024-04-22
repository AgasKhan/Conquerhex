using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGiver : LogicActive<Character>
{
    public List<Ingredient> items = new List<Ingredient>();

    public override void Activate(Character genericParams)
    {
        foreach (var item in items)
        {
            genericParams.inventory.AddItem(item.Item, item.Amount);
        }
    }
}
