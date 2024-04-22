using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class InventoryEntityComponent : ComponentOfContainer<Entity>,IEnumerable<Item> , ISaveObject //, IItemContainer
{
    public event System.Action<InventoryEntityComponent> onChangeDisponiblity;

    [SerializeField]
    public List<Vector2Int> visualItems = new List<Vector2Int>();
    
    [SerializeField]
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

    #region No implementadas

    public void AddOrSubstractItems(string str, int index)
    {
        throw new System.NotImplementedException("Tenes que borrar la funcion de add or substract capo, o si no implementarla, digo, deja de perder el tiempo");
    }

    #endregion


    public bool Contains(Item item)
    {
        return inventory.Contains(item);
    }

    public void Clear()
    {
        inventory.Clear();
        visualItems.Clear();
    }

    public int ItemCount<T>(T item) where T : System.IComparable<Item>
    {
        if (inventory.Contains(item, out int index))
        {
            return inventory[index].GetCount();
        }
        else
            return -1;
    }

    /// <summary>
    /// No implementada
    /// </summary>
    /// <param name="item"></param>
    /// <param name="inventory"></param>
    public void SubstractItem(Item item, InventoryEntityComponent inventory)
    {
        if (HasCapacity(item))
        {
            item.ChangeContainer(inventory);
        }
    }

    public virtual void AddAllItems(InventoryEntityComponent entity)
    {
        if (entity.CurrentWeight + CurrentWeight > WeightCapacity)
            return;

        AddAllItems(entity.inventory);
        entity.inventory.Clear();
    }

    protected void AddAllItems(OrderedList<Item> items)
    {
        Debug.Log(string.Join("", inventory));

        for (int i = items.Count - 1; i >= 0; i--)
        {
            AddItem(items[i]);
        }
    }

    /// <summary>
    /// Crea un item directamente en el inventario
    /// </summary>
    /// <param name="itemBase"></param>
    /// <param name="count"></param>
    public void AddItem(ItemBase itemBase, int count)
    {
        if (!HasCapacity(count,itemBase))
            return;

        CurrentWeight += itemBase.weight * count;

        Item item;

        var aux = inventory.BinarySearch(itemBase);

        if(aux < 0)
        {
           item = itemBase.Create();
           aux = item.Init(this);
        }
        else
        {
            item = inventory[aux];
        }

        //obtengo el indice del stack a agregar
        int indexStack = item.GetStackCount();

        if (itemBase.maxAmount>1)
        {
            //voy creando los stacks necesarios
            while(count>0)
            {
                item.AddAmount(-1, count, out count);
                AddVisualItem(0, aux, indexStack);
                indexStack++;
            }
        }            
        else
        {
            //para el primero creado
            count--;

            AddVisualItem(0, aux, 0);

            //Si necesito crear mas
            while (count > 0)
            {
                //por cada creado le resto uno a la cantidad faltante
                count--;
                AddVisualItem(0, itemBase.Create().Init(this), 0);
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

        int index;

        if (item is ItemStackeable && inventory.Contains(item, out index))
        {
            int indexStack = inventory[index].GetStackCount()-1;

            for (int j = 0; j < item.GetStackCount(); j++)
            {
                item.GetAmounts(j, out int actual);

                inventory[index].AddAmount(-1, actual, out int resto);
            }

            AddVisualCompleteItem(item, index, indexStack);

            item.Destroy();
        }
        else
            item.ChangeContainer(this);
    }

    #region internals

    /// <summary>
    /// Funcion que sera llamada de forma automatica por el Item <br/>
    /// NO UTILIZAR PARA OTROS FINES
    /// </summary>
    /// <param name="item"></param>
    public int InternalAddItem(Item item)
    {
        if (!(item is Resources_Item))
        {
            var orderedIndex = inventory.Add(item);
            if (item.visible)
                visualItems.Add(new Vector2Int(orderedIndex, -1));

            return orderedIndex;     
        }

        if (!inventory.Contains(item, out int indx))
        {
            int orderedIndex = inventory.Add(item);

            for (int i = 0; i < item.GetStackCount(); i++)
            {
                visualItems.Add(new Vector2Int(orderedIndex, i));
            }

            return orderedIndex;
        }
        else
        {
            for (int i = 0; i < item.GetStackCount(); i++)
            {
                item.GetAmounts(i, out int actual);
                inventory[indx].AddAmount(-1, actual, out int rst);

                visualItems.Add(new Vector2Int(indx, inventory[indx].GetStackCount() - 1));
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

        if (!item.visible)
        {
            return indexOrder;
        }

        int aux = visualItems.Count - 1;
        for (; aux >= 0; aux--)
        {
            if (visualItems[aux].x == indexOrder)
            {
                visualItems.RemoveAt(aux);
                if (!(item is Resources_Item))
                    break;
            }
        }

        RefreshOrderedList(indexOrder,-1);
        return indexOrder;
    }

    #endregion

    #region VisualItems

    /// <summary>
    /// Funcion dedicada a actualizar el indice de la representacion de las casillas
    /// </summary>
    /// <param name="orderIndex"></param>
    /// <param name="operation"></param>
    void RefreshOrderedList(int orderIndex, int operation)
    {
        for (int i = 0; i < visualItems.Count - 1; i++)
        {
            if (visualItems[i].x > orderIndex)
                visualItems[i] = new Vector2Int(visualItems[i].x + operation, visualItems[i].y);
        }
    }

    /// <summary>
    /// Funcion para agregar todas las representaciones en casillas de un item y su contenido
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <param name="indexStack"></param>
    void AddVisualCompleteItem(Item item, int index, int indexStack)
    {
        RefreshOrderedList(index,+1);

        if(item is ItemStackeable)
        {
            for (int i = indexStack; i < item.GetStackCount(); i++)
            {
                AddVisualItem(0, index, i);
            }
        }
        else
        {
            AddVisualItem(0, index, -1);
        }
    }

    /// <summary>
    /// Funcion para agregar una representacion en la casilla
    /// </summary>
    /// <param name="indexOrderList"></param>
    /// <param name="index"></param>
    /// <param name="indexStack"></param>
    void AddVisualItem(int indexOrderList, int index, int indexStack)
    {
        visualItems.Add(new Vector2Int(index, indexStack));
    }

    /// <summary>
    /// Funcion para remover una representacion en la casilla
    /// </summary>
    /// <param name="indexOrderList"></param>
    /// <param name="index"></param>
    /// <param name="indexStack"></param>
    void RemoveVisualItem(int indexOrderList)
    {
        visualItems.RemoveAt(indexOrderList);
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

            if (_indexEquipedItem >= 0 && _indexEquipedItem < inventoryComponent.Count)
            {
                _equiped = inventoryComponent[_indexEquipedItem];
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