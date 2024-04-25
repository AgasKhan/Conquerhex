using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGiver : LogicActive<Entity>
{
    public List<Ingredient> items = new List<Ingredient>();

    public override void Activate(Entity genericParams)
    {
        foreach (var item in items)
        {
            genericParams.GetInContainer<InventoryEntityComponent>().AddItem(item.Item, item.Amount);
        }
    }
}
