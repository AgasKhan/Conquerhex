using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class InventoryEntityComponent : ComponentOfContainer<Entity> //, IItemContainer
{
    //public Pictionarys<string,LogicActive> interact; //funciones de un uso para la interaccion

    [SerializeReference]
    public List<Item> inventory = new List<Item>();

    public virtual float weightCapacity { get; }

    public float currentWeight = 0f;

    //Pictionarys<string, LogicActive> actions; //funciones de un uso para cuestiones internas

    public List<Timer> travelItem = new List<Timer>();

    public override void OnEnterState(Entity param)
    {
    }

    public override void OnStayState(Entity param)
    {
    }

    public override void OnExitState(Entity param)
    {
        container = null;
    }

    public bool Contains(Item item)
    {
        return inventory.Contains(item);
    }

    public virtual void AddAllItems(InventoryEntityComponent entity)
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

            inventory[inventory.Count - 1].AddAmount(amount - 1, out amount);

            AddOrCreate(itemBase, amount);
        }
    }
}

[System.Serializable]
public class SlotItem<T> where T : Item
{
    [System.NonSerialized]
    public InventoryEntityComponent inventoryComponent;

    public event System.Action<int, T> toChange;

    [SerializeField]
    int _indexEquipedItem = -1;

    [SerializeReference]
    T _equiped;

    public T equiped
    {
        get
        {
            if (inventoryComponent == null || !(_equiped?.HaveSameContainer(inventoryComponent) ?? false))
                return default;
            else
                return _equiped;
        }
    }

    public int indexEquipedItem
    {
        //get => _indexEquipedItem;
        set
        {
            _indexEquipedItem = value;

            if(_indexEquipedItem >= 0 && _indexEquipedItem < inventoryComponent.inventory.Count)
            {
                _equiped = inventoryComponent.inventory[_indexEquipedItem] as T;
            }
            else
            {
                _equiped = null;
            }

            Debug.Log("Equipado : " + _equiped);
            
            toChange?.Invoke(_indexEquipedItem, equiped);
        }
    }
}


[System.Serializable]
public class SlotItemList<T> where T : Item
{
    [SerializeField]
    SlotItem<T>[] list;

    public int Count => list.Length;

    public SlotItem<T> this[int index]
    {
        set
        {
            list[index] = value;
        }
        get
        {
            return list[index];
        }
    }



    [field: SerializeField]
    public int indexer { get; protected set; }

    public SlotItem<T> actual
    {
        get
        {
            return list[indexer];
        }
        set
        {
            list[indexer] = value;
        }
    }

    public SlotItem<T> Actual(int index)
    {
        indexer = index;
        return list[indexer];
    }

    public void Next()
    {
        indexer++;

        if (indexer >= list.Length)
            indexer = 0;
    }

    public void Previus()
    {
        indexer--;

        if (indexer < 0)
            indexer = list.Length - 1;
    }

    public void Init(InventoryEntityComponent inventoryEntityComponent)
    {
        for (int i = 0; i < Count; i++)
        {
            list[i] = new SlotItem<T>();
            list[i].inventoryComponent = inventoryEntityComponent;
        }
    }

    public SlotItemList(int number)
    {
        list = new SlotItem<T>[number];
    }
}