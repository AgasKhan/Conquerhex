using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItem
{
    [SerializeField]
    public bool isModifiable = true;

    [System.NonSerialized]
    public InventoryEntityComponent inventoryComponent;

    [SerializeField]
    protected int _indexEquipedItem = -1;

    [field:SerializeField]
    public int indexSlot { get; protected set; } = -1;

    [SerializeReference]
    ItemEquipable _equiped;

    [SerializeReference]
    ItemEquipable defaultItem;

    public ItemEquipable equiped
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
        set
        {  
            if (equiped != null)
            {
                equiped.onDrop -= EquipedOnDrop;
                equiped.equipedSlot = null;
                equiped.Unequip();
            }

            _indexEquipedItem = value;

            if (_indexEquipedItem >= 0 && _indexEquipedItem < inventoryComponent.Count)
            {
                _equiped = (ItemEquipable)inventoryComponent[_indexEquipedItem];
                
                if (!isModifiable)
                    ((Ability)_equiped).original.equipedSlot = this;
                
                _equiped.onDrop += EquipedOnDrop;
                _equiped.equipedSlot = this;
                _equiped.Equip(indexSlot);
            }
            else
            {
                _equiped = null;

                if(defaultItem != null)
                {
                    inventoryComponent.Contains(defaultItem, out int index);
                    indexEquipedItem = index;
                }
            }
        }
    }

    public void SetDefaultItem(ItemEquipable _item)
    {
        defaultItem = _item;
    }

    protected virtual void EquipedOnDrop()
    {
        _equiped.onDrop -= EquipedOnDrop;
        _indexEquipedItem = -1;
        _equiped = null;
    }
}

[System.Serializable]
public class SlotItem<T> : SlotItem where T : ItemEquipable
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
            base.indexEquipedItem = value;
            toChange?.Invoke(_indexEquipedItem, equiped);
        }
    }

    protected override void EquipedOnDrop()
    {
        base.EquipedOnDrop();
        toChange?.Invoke(-1, null);
    }

    public SlotItem()
    {
    }

    public SlotItem(int indexSlot)
    {
        base.indexSlot = indexSlot;
    }
}


[System.Serializable]
public class SlotItemList<T> : IEnumerable<T> where T : ItemEquipable
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
                if (this[i].equiped != null)
                    return false;
            }

            return true;
        }
    }

    public SlotItem<T> this[int index]
    {
        get
        {
            return list[index];
        }
        private set
        {
            list[index] = value;
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
    }

    public SlotItem<T> Actual(int index)
    {
        if (index == indexer)
            return actual;

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
        while (actual.equiped == null && !empty);
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
        while (actual.equiped == null && !empty);
    }

    public void Init(InventoryEntityComponent inventoryEntityComponent, System.Action<SlotItemList<T>, int, int, T> toChangeEquipament = null)
    {
        for (int i = 0; i < Count; i++)
        {
            int slot = i;
            list[i] = new SlotItem<T>(i);
            list[i].inventoryComponent = inventoryEntityComponent;

            if (toChangeEquipament != null)
                list[i].toChange += (indexItem, item)=> toChangeEquipament(this,slot,indexItem,item);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in list)
        {
            yield return item.equiped;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public SlotItemList(int number)
    {
        list = new SlotItem<T>[number];
    }
}
