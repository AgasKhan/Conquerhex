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

    [SerializeField]
    protected int _indexSlot = -1;

    [SerializeReference]
    ItemEquipable _equiped;

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
                _equiped.Equip(_indexSlot);
            }
            else
            {
                _equiped = null;
            }
        }
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
        _indexSlot = indexSlot;
    }
}


[System.Serializable]
public class SlotItemList<T> where T : ItemEquipable
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

    public void Init(InventoryEntityComponent inventoryEntityComponent)
    {
        for (int i = 0; i < Count; i++)
        {
            list[i] = new SlotItem<T>(i);
            list[i].inventoryComponent = inventoryEntityComponent;
        }
    }

    public SlotItemList(int number)
    {
        list = new SlotItem<T>[number];
    }
}
