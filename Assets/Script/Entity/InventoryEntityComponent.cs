using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class InventoryEntityComponent : ComponentOfContainer<Entity>,IEnumerable<Item> , ISaveObject //, IItemContainer
{
    #region test

    [SerializeField]
    ItemBase itemBase;

    [SerializeField]
    int count=1;

    [ContextMenu("Add")]
    void Test1()
    {
        AddItem(itemBase, count);
    }

    [ContextMenu("Remove")]
    void Test2()
    {
        SubstracItems(itemBase, count);
    }

    #endregion


    public event System.Action<InventoryEntityComponent> onChangeDisponiblity;
    
    public event System.Action<Item> onNewItem;
    public event System.Action<Item> onLostItem;


    OrderedList<Item> inventory = new OrderedList<Item>();

    [SerializeField]
    float _currentWeight = 0f;

    [SerializeField]
    BodyBase flyweight;

    public int Count => inventory.Count;

    public virtual float WeightCapacity => flyweight.weightCapacity;

    public float CurrentWeight
    {
        get => _currentWeight;
        set
        {
            _currentWeight = value;
            onChangeDisponiblity?.Invoke(this);
        }
    }

    public Item this[int index]
    {
        get
        {
            return inventory[index];
        }
    }

    public bool Contains(Item item)
    {
        return inventory.Contains(item);
    }

    public bool Contains(Item item, out int index)
    {
        return inventory.Contains(item, out index);
    }

    public bool Contains(string name, out Item item, out int index)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if(inventory[i].GetItemBase().nameDisplay == name)
            {
                if(inventory[i] is Ability && ((Ability)inventory[i]).IsCopy)
                    item = ((Ability)inventory[i]).original;
                else
                    item = inventory[i];

                Debug.Log("SI se encontro el item: " + name);

                index = i;
                return true;
            }
        }
        Debug.Log("NO se encontro el item: " + name);
        index = -1;
        item = null;
        return false;
    }


    public void Clear()
    {
        inventory.Clear();
    }

    public int ItemCount<T>(T item) where T : System.IComparable<Item>
    {
        return ItemCount(item, out int startIndex, out int endIndex);
    }

    public int ItemCount<T>(T item, out int startIndex, out int endIndex) where T : System.IComparable<Item>
    {
        if (inventory.Contains(item, out startIndex, out endIndex))
        {
            if(startIndex==endIndex)
                return inventory[startIndex].GetCount();
            else
            {
                int count = 0;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    count += inventory[i].GetCount();
                }
                return count;
            }
                
        }  
        else
            return -1;
    }

    public void SubstracItems(ItemBase itemToCompare, int count)
    {
        int countInInventory = ItemCount(itemToCompare, out int startIndex, out int endIndex);

        if (countInInventory < count)
        {
            Debug.Log("Item invalido para ser removido");
            return;
        }

        CurrentWeight -= itemToCompare.weight * count;
        
        //if (typeof(ItemStackeable).IsAssignableFrom(itemBase.GetItemType()))
        if (itemToCompare is ResourcesBase_ItemBase)//Version de items stackeables
        {
            if (count > 0)
                count = -count;

            Item item = inventory[startIndex];

            int resto = count;

            while (resto < 0 && item.GetStackCount()!=0)
            {
                item.AddAmount(-1, resto, out resto);
            }

            if (item.GetStackCount() == 0)
            {
                inventory.Remove(item);
                onLostItem?.Invoke(item);
            }
        }
        else //Version de items no stackeables
        {
            if (count < 0)
                count = -count;

            Item[] items = new Item[endIndex - startIndex];

            inventory.CopyTo(0,items, startIndex, items.Length);

            for (int i = 0; i < items.Length && count>0; i++)
            {
                inventory.Remove(items[i]);
                onLostItem?.Invoke(items[i]);
                count--;
            }
        }        
    }

    public virtual void AddAllItems(InventoryEntityComponent entity)
    {
        if (entity.CurrentWeight + CurrentWeight > WeightCapacity)
            return;

        for (int i = entity.inventory.Count - 1; i >= 0; i--)
        {
            entity.inventory[i].ChangeContainer(this);
        }

        Debug.Log(string.Join("", inventory));
        entity.inventory.Clear();
    }

    /// <summary>
    /// Crea un item directamente en el inventario
    /// </summary>
    /// <param name="itemBase"></param>
    /// <param name="count"></param>
    public void AddItem(ItemBase itemBase, int count)
    {
        if (!HasCapacity(count,itemBase) || count<=0)
            return;

        //Debug.Log("ADD\nSE AGREGO EL ITEM: " + itemBase.nameDisplay + "\nCANTIDAD: " + count);

        CurrentWeight += itemBase.weight * count;

        Item item;

        if (typeof(ItemStackeable).IsAssignableFrom(itemBase.GetItemType()))
        {
            //obtengo el indice del stack a agregar
            var aux = inventory.BinarySearch(itemBase);

            if (aux < 0)
            {
                //para el primero creado
                item = itemBase.Create();
                item.Init(this);
                onNewItem?.Invoke(item);
            }
            else
            {
                item = inventory[aux];
            }

            //voy creando los stacks necesarios
            while (count > 0)
            {
                item.AddAmount(-1, count, out count);
            }
        }            
        else
        {
            //Si necesito crear mas
            while (count > 0)
            {
                //por cada creado le resto uno a la cantidad faltante
                count--;
                item = itemBase.Create();
                item.Init(this);
                onNewItem?.Invoke(item);
            }
        }
    }

    /// <summary>
    /// Aniade un item de otro inventario
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        if (!HasCapacity(item))
            return;

        CurrentWeight += item.GetItemBase().weight * item.GetCount();

        if (item is ItemStackeable && inventory.Contains(item, out int index))
        {
            //Debug.Log("Es stackeable");
            int indexStack = inventory[index].GetStackCount()-1;

            for (int j = 0; j < item.GetStackCount(); j++)
            {
                item.GetAmounts(j, out int actual);

                inventory[index].AddAmount(indexStack, actual, out int resto);
            }

            item.Destroy();
        }
        else
        {
            //Debug.Log("No es stackeable");
            item.ChangeContainer(this);
        }
            
    }

    #region internals

    /// <summary>
    /// Funcion que sera llamada de forma automatica por el Item <br/>
    /// NO UTILIZAR PARA OTROS FINES
    /// </summary>
    /// <param name="item"></param>
    public int InternalAddItem(Item item)
    {
        if(!(item is ItemStackeable) || !inventory.Contains(item, out int indx))
        {
            int orderedIndex = inventory.Add(item);
            onNewItem?.Invoke(item);
            return orderedIndex;
        }
        else
        {
            for (int i = 0; i < item.GetStackCount(); i++)
            {
                item.GetAmounts(i, out int actual);
                inventory[indx].AddAmount(-1, actual, out int rst);
            }
            return indx;
        }
    }

    /// <summary>
    /// Funcion que sera llamada de forma automatica por el Item <br/>
    /// NO UTILIZAR PARA OTROS FINES
    /// </summary>
    /// <param name="item"></param>
    public int InternalRemoveItem(Item item)
    {
        int indexOrder = inventory.Remove(item);

        if (indexOrder == -1)
        {
            Debug.LogError("No contiene el item");
            return -1;
        }

        onLostItem?.Invoke(item);

        return indexOrder;
    }

    #endregion



    #region chequeo de capacidad

    public bool HasCapacity(int count, ItemBase items)
    {
        return items.weight * count + CurrentWeight < WeightCapacity;
    }

    public bool HasCapacity(params Item[] items)
    {
        float totalWeight = 0;
        foreach (var item in items)
        {
            if (item is Resources_Item)
            {
                var resource = item as Resources_Item;
                for (int i = 0; i < resource.GetStackCount(); i++)
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

        return (CurrentWeight + totalWeight) < WeightCapacity;
    }

    #endregion

    #region interfaces

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return ((IEnumerable<Item>)inventory).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)inventory).GetEnumerator();
    }

    #endregion

    #region seteo

    public override void OnEnterState(Entity param)
    {
        flyweight = param.flyweight?.GetFlyWeight<BodyBase>();
    }

    public override void OnStayState(Entity param)
    {
    }

    public override void OnExitState(Entity param)
    {
        container = null;
        flyweight = null;
    }
    #endregion
}

