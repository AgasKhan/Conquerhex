using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour, IItemContainer
{
    StaticEntity customer;

    private void Awake()
    {
        customer = GetComponent<StaticEntity>();
    }

    public void AddItems(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            customer.inventory.Add(item);
        }
    }
    public void RemoveItems(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            customer.inventory.Remove(item);
        }
    }

    public bool ContainsItem(Item item)
    {
        return customer.inventory.Contains(item);
    }

    public int ItemCount(Item item)
    {
        throw new NotImplementedException();
    }

}



public interface IItemContainer
{
    bool ContainsItem(Item item);
    int ItemCount(Item item);
    void RemoveItems(Item item, int amount);
    void AddItems(Item item, int amount);
}
