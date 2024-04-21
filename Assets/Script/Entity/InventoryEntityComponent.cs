using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class InventoryEntityComponent : ComponentOfContainer<Entity>, ISaveObject //, IItemContainer
{
    [SerializeReference]
    public List<(string name, int index)> visualItems = new List<(string,int)>();
    
    [SerializeField]
    public OrderedList<Item> inventory = new OrderedList<Item>();

    public virtual float weightCapacity => container.flyweight.GetFlyWeight<BodyBase>().weightCapacity;

    public float currentWeight = 0f;

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

    protected List<Item> AddAllItems(List<Item> items)
    {
        Debug.Log(string.Join("", inventory));

        for (int i = items.Count - 1; i >= 0; i--)
        {
            items[i].ChangeContainer(this);
            currentWeight += items[i].GetItemBase().weight;
            items.Remove(items[i]);
        }

        return items;
    }

    public int ItemCount(string itemName)
    {
        int amount = 0;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].nameDisplay == itemName)
            {
                //inventory[i].GetAmounts(out int actual, out int max);
                //amount += actual;
            }
        }
        return amount;
    }


    /// <summary>
    /// Funcion que sera llamada de forma automatica por el Item <br/>
    /// NO UTILIZAR PARA OTROS FINES
    /// </summary>
    /// <param name="item"></param>
    public void InternalAddItem(Item item)
    {
        if (item is Resources_Item)
        {
            if (!inventory.Contains(item, out int indx))
            {
                inventory.Add(item);

                for (int i = 0; i < item.GetCount(); i++)
                {
                    visualItems.Add((item.nameDisplay, i));
                }
            }
            else
            {
                for (int i = 0; i < item.GetCount(); i++)
                {
                    item.GetAmounts(i, out int actual);
                    inventory[indx].AddAmount(-1, actual, out int rst);

                    visualItems.Add((item.nameDisplay, inventory[indx].GetCount() - 1));
                }
            }
        }
        else
        {
            var aux = inventory.Add(item);
            if (item.visible)
                visualItems.Add((item.nameDisplay, aux));
        }
    }

    /// <summary>
    /// Funcion que sera llamada de forma automatica por el Item <br/>
    /// NO UTILIZAR PARA OTROS FINES
    /// </summary>
    /// <param name="item"></param>
    public void InternalRemoveItem(Item item)
    {
        if (item is Resources_Item)
        {
            if (inventory.Remove(item) == -1)
            {
                Debug.LogError("No contiene el item");
            }
            else
            {
                for (int i = visualItems.Count - 1; i >= 0; i--)
                {
                    if (visualItems[i].name == item.nameDisplay)
                        visualItems.RemoveAt(i);
                }
            }
        }
        else
        {
            var aux = inventory.Remove(item);
            if (item.visible && aux == -1)
            {
                Debug.LogError("No contiene el item");
                return;
            }

            List<int> buffer = new List<int>();
            for (int i = visualItems.Count - 1; i >= 0; i--)
            {
                if (visualItems[i].name == item.nameDisplay)
                    buffer.Add(i);
            }

            foreach (var number in buffer)
            {
                if (visualItems[number].index == aux)
                {
                    visualItems.RemoveAt(number);
                    buffer.Remove(number);
                    break;
                }
            }

            if (inventory.Contains(item, out int start, out int end))
            {
                var indexer = start;

                foreach (var number in buffer)
                {
                    visualItems[number] = (item.nameDisplay, inventory.IndexOf(inventory[indexer]));
                    indexer++;
                }
            }
        }
    }
    public void AddItem(Item item)
    {
        if(HasCapacity (container.flyweight.weight, container.flyweight.GetFlyWeight<BodyBase>().weightCapacity, item))
        {
            item.ChangeContainer(this);
        }
    }

    public void SubstractItem(Item item, InventoryEntityComponent inventory)
    {
        if (HasCapacity(inventory.container.flyweight.weight, inventory.container.flyweight.GetFlyWeight<BodyBase>().weightCapacity, item))
        {
            item.ChangeContainer(inventory);
        }
    }

    bool HasCapacity(float actualWeight, float weightCapacity, params Item[] items)
    {
        float totalWeight = 0;
        foreach (var item in items)
        {
            if (item is Resources_Item)
            {
                var resource = item as Resources_Item;
                for (int i = 0; i < resource.GetCount(); i++)
                {
                    resource.GetAmounts(i, out int actual);
                    totalWeight += resource.itemBase.weight * actual;
                }
            }
            else
            {
                totalWeight = item.GetItemBase().weight;
            }
        }

        return (actualWeight+totalWeight) < weightCapacity;
    }

    public void AddOrSubstractItems(string itemName, int amount)
    {
        /*
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
        */
        //Debug.Log(string.Join("", inventory));
    }


    void AddOrCreate(ItemBase itemBase, int amount)
    {
        /*
        if (amount > 0)
        {
            inventory.Add(itemBase.Create());

            inventory[^1].Init(this);
            
            inventory[inventory.Count - 1].AddAmount(amount - 1, out amount);

            AddOrCreate(itemBase, amount);
        }
        */
    }

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
    }
}

public class SlotItem
{
    [System.NonSerialized]
    public InventoryEntityComponent inventoryComponent;

    [SerializeField]
    protected int _indexEquipedItem = -1;

    [SerializeReference]
    Item _equiped;

    public Item equiped
    {
        get
        {
            if (inventoryComponent == null || !(_equiped?.HaveSameContainer(inventoryComponent) ?? false))
                return default;
            else
                return _equiped;
        }
    }

    public virtual int indexEquipedItem
    {
        //get => _indexEquipedItem;
        set
        {
            _indexEquipedItem = value;

            if (_indexEquipedItem >= 0 && _indexEquipedItem < inventoryComponent.inventory.Count)
            {
                _equiped = inventoryComponent.inventory[_indexEquipedItem];
            }
            else
            {
                _equiped = null;
            }

            Debug.Log("Equipado : " + _equiped);

        }
    }

}

[System.Serializable]
public class SlotItem<T>: SlotItem where T : Item
{
    public event System.Action<int, T> toChange;

    public new T equiped
    {
        get
        {
            return base.equiped as T;
        }
    }

    public override int indexEquipedItem 
    { 
        set
        {
            if (equiped != null)
                equiped.onDrop -= EquipedOnDrop;

            base.indexEquipedItem = value;
            toChange?.Invoke(_indexEquipedItem, equiped);

            if(equiped!=null)
                equiped.onDrop += EquipedOnDrop;
        } 
    }

    private void EquipedOnDrop()
    {
        indexEquipedItem = -1;
    }
}


[System.Serializable]
public class SlotItemList<T> where T : Item
{
    [SerializeField]
    SlotItem<T>[] list;

    public int Count => list.Length;

    /// <summary>
    /// Verifica si esta vacio, recorriendo la lista entera y chequeando si tiene algo equipado
    /// </summary>
    public bool Empty
    {
        get
        {
            for (int i = 0; i < Count; i++)
            {
                if(this[i].equiped != null)
                    return false;
            }

            return true;
        }
    }

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
        bool empty = Empty;

        do
        {
            indexer++;

            if (indexer >= list.Length)
                indexer = 0;
        }
        while(actual.equiped == null && !empty);
    }

    public void Previus()
    {
        bool empty = Empty;

        do
        {
            indexer--;

            if (indexer < 0)
                indexer = list.Length - 1;
        }
        while(actual.equiped == null && !empty);
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