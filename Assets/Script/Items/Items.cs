using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemBase : ShowDetails, IComparable<ItemBase>, IComparable<Item>
{
    [Header("Items")]
    [Range(1, 1000)]
    public int maxAmount = 1;

    public float weight;

    public Dictionary<string, System.Action<Character, Item>> buttonsAcctions = new Dictionary<string, System.Action<Character, Item>>();

    System.Type _itemType;

    public bool visible = true;

    public abstract System.Type GetItemType();

    public virtual Item Create()
    {
        var aux = System.Activator.CreateInstance(_itemType) as Item;

        aux.SetItemBase(this);

        return aux;
    }
    
    public int CompareTo(Item obj)
    {
        if (obj == null)
            return 1;

        return string.Compare(nameDisplay, obj.nameDisplay, StringComparison.Ordinal);
    }

    public int CompareTo(ItemBase other)
    {
        if (other == null)
            return 1;

        return string.Compare(nameDisplay, other.nameDisplay, StringComparison.Ordinal);
    }

    protected override void MyDisable()
    {
        Manager<ItemBase>.pic.Remove(nameDisplay);
    }

    protected override void MyEnable()
    {
        _itemType = GetItemType();
        Manager<ItemBase>.pic.Add(nameDisplay, this);
        buttonsAcctions.Clear();
        CreateButtonsAcctions();
    }

    protected virtual void CreateButtonsAcctions()
    {
        buttonsAcctions.Add("Destroy", DestroyItem);
    }

    protected virtual void DestroyItem(Character character, Item item)
    {
        character.inventory.InternalRemoveItem(item);
    }
}


[System.Serializable]
public abstract class Item : IShowDetails, IComparable<Item>, IComparable<ItemBase>
{
    public event System.Action onDrop;//si ejecuto el ondrop, este desequipa el item
    public event System.Action<InventoryEntityComponent> OnBeforeChangeContainer;
    public event System.Action<InventoryEntityComponent> OnAfterChangeContainer;
    
    [field: SerializeField]
    protected InventoryEntityComponent container { get; private set; }

    [SerializeField]
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public System.Type itemType => GetType();

    public virtual bool visible => _itemBase.visible;

    protected abstract void Init();

    public abstract Item SetItemBase(object baseItem);

    public override string ToString()
    {
        return nameDisplay + "\n\n" + GetDetails().ToString(": ", "\n") + "\n";
    }

    public override bool Equals(object obj)
    {
        Debug.Log("Original: "+nameDisplay + "  Comparador: " +  obj.GetType().Name);

        if (obj is Item)
            return ((Item)obj).nameDisplay == nameDisplay;
        else
            return base.Equals(obj);
    }

    public virtual void Destroy()
    {
        onDrop?.Invoke();

        container.InternalRemoveItem(this);

        container = null;
    }

    public virtual Pictionarys<string, string> GetDetails()
    {
        return _itemBase.GetDetails();
    }

    public virtual Item Create()
    {
        var aux = _itemBase.Create();
        return aux;
    }

    public virtual void GetAmounts(int index, out int actual)
    {
        actual = 1;
    }

    public virtual int GetStackCount()
    {
        return 1;
    }

    public virtual int GetCount()
    {
        return 1;
    }

    public int GetCount(out int maxAmountCount)
    {
        maxAmountCount = _itemBase.maxAmount;
        return GetCount();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>Cuantos items me sobraron desp de apilarlos</returns>
    public virtual Item AddAmount(int index, int amount, out int resto)
    {
        resto = amount;

        return this;
    }

    public ItemBase GetItemBase()
    {
        return _itemBase;
    }

    public int CompareTo(Item obj)
    { 
        return _itemBase.CompareTo(obj);
    }

    public int CompareTo(ItemBase other)
    {
        return _itemBase.CompareTo(other);
    }

    public bool HaveSameContainer(InventoryEntityComponent container)
    {
        return container == this.container;
    }

    public bool HaveSameContainer(Item item)
    {
        return item.container == this.container;
    }

    public void ChangeContainer(InventoryEntityComponent inventoryEntityComponent)
    {
        if (inventoryEntityComponent == this.container)
            return;

        //onDrop?.Invoke();

        OnBeforeChangeContainer?.Invoke(inventoryEntityComponent);

        Destroy();

        Init(inventoryEntityComponent);

        OnAfterChangeContainer?.Invoke(inventoryEntityComponent);
    }

    public int Init(InventoryEntityComponent inventoryEntityComponent)
    {
        if (container != null)
            return -1;

        container = inventoryEntityComponent;
        var aux = container.InternalAddItem(this);
        Init();

        return aux;
    }


}

[System.Serializable]
public abstract class ItemStackeable : Item
{
    [SerializeField]
    List<int> stacks = new List<int>();

    [SerializeField]
    int _count;

    public override Item AddAmount(int index, int amount, out int resto)
    {
        if(index < 0)
        {
            if(amount > 0)
            {
                stacks.Add(0);
            }

            index = stacks.Count - 1;
        }

        stacks[index] += amount;

        resto = 0;

        if (stacks[index] > _itemBase.maxAmount)
        {
            resto = stacks[index] - _itemBase.maxAmount;
            stacks[index] = _itemBase.maxAmount;
        }
        else if (stacks[index] <= 0)
        {
            resto = stacks[index];
            stacks.RemoveAt(index);
        }

        _count += amount - resto;


        return this;
    }

    public override void GetAmounts(int index, out int actual)
    {
        actual = stacks[index];
    }

    public override int GetStackCount()
    {
        return stacks.Count;
    }

    public override int GetCount()
    {
        return _count;
    }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();
        //aux.Add("Cantidad", actual + " / "+ itemBase.maxAmount);

        return aux;
    }
}

[System.Serializable]
public abstract class ItemEquipable : Item
{
    public SlotItem equipedSlot = null;

    public bool isEquiped => equipedSlot != null;

    public bool isRemovable => isEquiped? equipedSlot.isModifiable : true;

    public event Action<int> OnEquipedInSlot;

    public void Equip(int slot)
    {
        OnEquipedInSlot?.Invoke(slot);
    }

    public virtual void Unequip()
    {

    }
}

[System.Serializable]
public abstract class Item<T> : Item where T : ItemBase
{
    public T itemBase
    {
        get => (T)GetItemBase();
        set => SetItemBase(value);
    }

    public override Item SetItemBase(object baseItem)
    {
        if (baseItem is T)
        {
            _itemBase = baseItem as T;
        }
        else
        {
            Debug.LogWarning("Type itembase failed");
        }


        return this;
    }
}

[System.Serializable]
public abstract class ItemEquipable<T> : ItemEquipable where T : ItemBase
{
    public T itemBase
    {
        get => (T)GetItemBase();
        set => SetItemBase(value);
    }

    public override Item SetItemBase(object baseItem)
    {
        if (baseItem is T)
        {
            _itemBase = baseItem as T;
        }
        else
        {
            Debug.LogWarning("Type itembase failed");
        }


        return this;
    }
}

[System.Serializable]
public abstract class ItemStackeable<T> : ItemStackeable where T : ItemBase
{
    public T itemBase
    {
        get => (T)GetItemBase();
        set => SetItemBase(value);
    }

    public override Item SetItemBase(object baseItem)
    {
        if (baseItem is T)
        {
            _itemBase = baseItem as T;
        }
        else
        {
            Debug.LogWarning("Type itembase failed");
        }


        return this;
    }
}

[System.Serializable]
public abstract class ItemCrafteable : ItemBase
{
    public Recipes recipe;
}

