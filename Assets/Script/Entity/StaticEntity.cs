using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticEntity : Entity //, IItemContainer
{
    public Pictionarys<string,LogicActive> interact; //funciones de un uso para la interaccion

    [SerializeReference]
    public List<Item> inventory = new List<Item>();

    public float weightCapacity;

    public float currentWeight = 0f;

    //Pictionarys<string, LogicActive> actions; //funciones de un uso para cuestiones internas

    public List<Timer> travelItem = new List<Timer>();

    public bool Contains(Item item)
    {
        return inventory.Contains(item);
    }

    public virtual void AddAllItems(StaticEntity entity)
    {
        AddAllItems(entity.inventory);
        entity.inventory.Clear();
    }

    protected void AddAllItems(List<Item> items)
    {
        //inventory.AddRange(items);
        Debug.Log(string.Join("", inventory));

        foreach (var item in items)
        {
            if (currentWeight + item.GetItemBase().weight <= weightCapacity)
            {
                item.GetAmounts(out int actual, out int max);
                AddOrSubstractItems(item.nameDisplay, actual);

                currentWeight += item.GetItemBase().weight;
            }
            else
            {
                foreach (var timer in travelItem)
                {
                    timer.Stop();
                }
            }
        }        
    }

    public int ItemCount(string itemName)
    {
        int amount = 0;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].nameDisplay == itemName)
            {
                inventory[i].GetAmounts(out int actual, out int max);
                amount += actual;
            }
        }
        return amount;
    }

    public void AddOrSubstractItems(string itemName, int amount)
    {
        ItemBase myItemBase = null;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (itemName == inventory[i].nameDisplay)
            {
                inventory[i].GetAmounts(out int actual, out int max);

                if((amount + actual)<=0)
                {
                    inventory.RemoveAt(i);
                    amount += actual;
                }
                else
                {
                    inventory[i].AddAmount(amount, out amount);
                    myItemBase = inventory[i].GetItemBase();
                }

                if (amount == 0)
                    return;
            }
        }

        if (myItemBase == null)
        {
            if (amount < 0)
            {
                Debug.LogWarning("No posees el item: " + itemName);
                return;
            }
            else
            {
                myItemBase = Manager<ItemBase>.pic[itemName];
            }
        }

        AddOrCreate(myItemBase, amount);

        //Debug.Log(string.Join("", inventory));
    }

    void AddOrCreate(ItemBase itemBase, int amount)
    {
        if (amount > 0)
        {
            inventory.Add(itemBase.Create());

            inventory[inventory.Count - 1].AddAmount(amount - 1,out amount);

            AddOrCreate(itemBase, amount);
        }
    }

}