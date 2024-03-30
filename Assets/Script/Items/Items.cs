using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemBase : ShowDetails
{
    [Header("Items")]
    [Range(1, 1000)]
    public int maxAmount = 1;

    public float weight;

    public Dictionary<string, System.Action<Character, Item>> buttonsAcctions = new Dictionary<string, System.Action<Character, Item>>();

    System.Type _itemType;

    public bool visible = true;
    
    public virtual Item Create()
    {
        var aux = System.Activator.CreateInstance(_itemType) as Item;

        aux.SetItemBase(this);

        return aux;
    }

    protected override void MyDisable()
    {
        Manager<ItemBase>.pic.Remove(nameDisplay);
    }

    protected override void MyEnable()
    {
        _itemType = SetItemType();
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
        character.inventory.AddOrSubstractItems(nameDisplay, -1);
    }

    protected abstract System.Type SetItemType();
}






[System.Serializable]
public abstract class Item : IShowDetails
{
    public event System.Action onDrop;
    public event System.Action<InventoryEntityComponent> onChangeContainer;

    [SerializeField]
    protected InventoryEntityComponent container;

    [SerializeField]
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public System.Type itemType => GetType();

    public virtual bool visible => _itemBase.visible;

    public bool HaveSameContainer(InventoryEntityComponent container)
    {
        return container == this.container;
    }

    public bool HaveSameContainer(Item item)
    {
        return item.container == this.container;
    }

    public virtual Pictionarys<string, string> GetDetails()
    {
        return _itemBase.GetDetails();
    }

    public void Init(InventoryEntityComponent inventoryEntityComponent)
    {
        if (container != null)
            return;

        container = inventoryEntityComponent;
        Init();
    }

    public void ChangeContainer(InventoryEntityComponent inventoryEntityComponent)
    {
        if (inventoryEntityComponent == this.container)
            return;

        onDrop?.Invoke();

        onChangeContainer?.Invoke(inventoryEntityComponent);
    }

    protected abstract void Init();

    public abstract Item SetItemBase(object baseItem);

    public virtual Item Create()
    {
        var aux = _itemBase.Create();
        aux.Init(container);
        return aux;
    }

    public virtual void GetAmounts(out int actual, out int max)
    {
        max = _itemBase.maxAmount;
        actual = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>Cuantos items me sobraron desp de apilarlos</returns>
    public virtual Item AddAmount(int amount, out int resto)
    {
        resto = amount;

        return this;
    }

    public ItemBase GetItemBase()
    {
        return _itemBase;
    }

    public override string ToString()
    {
        return nameDisplay + "\n\n" + GetDetails().ToString(": " ,"\n") + "\n";
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
        if(baseItem is T)
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
public abstract class ItemStackeable<T> : Item<T> where T : ItemBase
{
    [SerializeField]
    int actual = 1;

    public override Item AddAmount(int amount, out int resto)
    {
        actual += amount;
        resto = 0;

        if (actual > itemBase.maxAmount)
        {
            resto = actual - itemBase.maxAmount;
            actual = itemBase.maxAmount;
        }
        else if (actual <= 0)
        {
            resto = actual;
        }

        return this;
    }

    public override void GetAmounts(out int actual, out int max)
    {
        max = itemBase.maxAmount;
        actual = this.actual;
    }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();
        aux.Add("Cantidad", actual + " / "+ itemBase.maxAmount);

        return aux;
    }
}

