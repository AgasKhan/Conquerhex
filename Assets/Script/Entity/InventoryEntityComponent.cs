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

    public int ItemCount(string item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].nameDisplay == item)
                return inventory[i].GetCount();
        }
        return -1;
    }

    /// <summary>
    /// Transfiere un Item desde otro inventario a este inventario <br/>
    /// Si la transferencia fue exitosa devuelve true, sino false
    /// </summary>
    /// <param name="item"></param>
    /*
    public bool TransferItem(Item item, InventoryEntityComponent destinyInventory)
    {
        if (HasCapacity(item))
        {
            item.ChangeContainer(this);
            return true;
        }

        return false;
    }
    */
    /// <summary>
    /// Quita una cantidad determinada de items stackeables
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param> Debe ser negativo, si se pasa un positivo se transformará
    public void SubstractStackItems(string itemStr, int count)
    {
        int orderIndex =-1;
        Item item = null;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].nameDisplay == itemStr)
            {
                orderIndex = i;
                item = inventory[i];
            }
                
        }

        var aux = orderIndex >= 0;

        if (!aux || item.GetCount() < count)
        {
            Debug.Log("Item invalido para ser removido");
            return;
        }

        if (count > 0)
            count = -count;

        int stackIndex = item.GetStackCount() - 1;
        int resto = count;

        while (resto < 0)
        {
            item.AddAmount(stackIndex, count, out resto);
            CurrentWeight += item.GetItemBase().weight * (count - resto);

            if (stackIndex == item.GetStackCount())
                RemoveVisualItem(orderIndex, stackIndex);

            stackIndex--;
        }

        if (item.GetStackCount() == 0)
            inventory.Remove(item);
    }

    public void Substracttems(ItemStackeable item, int count)
    {
        var aux = inventory.Contains(item, out int orderIndex);

        if (!aux || item.GetCount() < count)
        {
            Debug.Log("Item invalido para ser removido");
            return;
        }

        if (count > 0)
            count = -count;

        int stackIndex = item.GetStackCount() - 1;
        int resto = count;

        while (resto < 0)
        {
            item.AddAmount(stackIndex, count, out resto);
            CurrentWeight += item.GetItemBase().weight * (count - resto);

            if (stackIndex == item.GetStackCount())
                RemoveVisualItem(orderIndex, stackIndex);

            stackIndex--;
        }

        if (item.GetStackCount() == 0)
            inventory.Remove(item);
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
        if (!HasCapacity(count,itemBase))
            return;

        //Debug.Log("ADD\nSE AGREGO EL ITEM: " + itemBase.nameDisplay + "\nCANTIDAD: " + count);

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

        if (itemBase.maxAmount > 1)
        {
            //voy creando los stacks necesarios
            while(count > 0)
            {
                item.AddAmount(-1, count, out count);
                AddVisualItem(aux, indexStack);
                indexStack++;
            }
        }            
        else
        {
            //para el primero creado
            count--;

            AddVisualItem(aux, 0);

            //Si necesito crear mas
            while (count > 0)
            {
                //por cada creado le resto uno a la cantidad faltante
                count--;

                var orderedIndex = itemBase.Create().Init(this);
                AddVisualItem(orderedIndex, 0);
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
        //Debug.Log("INTERNAL ADD\nSE AGREGO EL ITEM: " + item.nameDisplay + "\nCANTIDAD: " + item.GetCount() +"\nDESTINATARIO: "+container.transform.gameObject.name);
        if (!(item is Resources_Item))
        {
            var orderedIndex = inventory.Add(item);
            if (item.visible)
                AddVisualItem(orderedIndex, 0);
            else
            {
                RefreshVisualList(orderedIndex, +1);
            }
            

            return orderedIndex;
        }

        if (inventory.Contains(item, out int indx))
        {
            for (int i = 0; i < item.GetStackCount(); i++)
            {
                item.GetAmounts(i, out int actual);
                inventory[indx].AddAmount(-1, actual, out int rst);

                AddVisualItem(indx, inventory[indx].GetStackCount() - 1);
            }

            return indx;
        }
        else
        {
            int orderedIndex = inventory.Add(item);

            for (int i = 0; i < item.GetStackCount(); i++)
            {
                AddVisualItem(orderedIndex, i);
            }

            return orderedIndex;
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

        RefreshVisualList(indexOrder,-1);
        return indexOrder;
    }

    #endregion

    #region VisualItems

    /// <summary>
    /// Funcion dedicada a actualizar el indice de la representacion de las casillas
    /// </summary>
    /// <param name="orderIndex"></param>
    /// <param name="operation"></param>
    void RefreshVisualList(int orderIndex, int operation)
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
        if(item is ItemStackeable)
        {
            for (int i = indexStack; i < item.GetStackCount(); i++)
            {
                AddVisualItem(index, i);
            }
        }
        else
        {
            AddVisualItem(index, -1);
        }
    }

    /// <summary>
    /// Funcion para agregar una representacion en la casilla
    /// </summary>
    /// <param name="orderedIndex"></param>
    /// <param name="indexStack"></param>
    /// <param name="indexBox"></param> Indice de la casilla donde es almacenado el item
    void AddVisualItem(int orderedIndex, int indexStack, int indexBox = 0)
    {
        visualItems.Add(new Vector2Int(orderedIndex, indexStack));
    }

    /// <summary>
    /// Funcion para remover una representacion en la casilla
    /// </summary>
    /// <param name="indexOrderList"></param>
    /// <param name="index"></param>
    /// <param name="indexStack"></param>
    void RemoveVisualItem(int indexOrderList, int indexStack)
    {
        visualItems.RemoveAt(SearchVisualItem(indexOrderList, indexStack));
    }

    int SearchVisualItem(int indexOrderList, int indexStack)
    {
        for (int i = 0; i < visualItems.Count; i++)
        {
            if (visualItems[i].x == indexOrderList && visualItems[i].y == indexStack)
                return i;
        }

        return -1;
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

