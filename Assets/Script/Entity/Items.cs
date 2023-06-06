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

    protected System.Type _itemType;
    protected abstract void SetCreateItemType();
    
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
        SetCreateItemType();
        Manager<ItemBase>.pic.Add(nameDisplay, this);
    }
}






[System.Serializable]
public abstract class Item : IShowDetails, Init
{
    [SerializeField]
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public virtual Pictionarys<string, string> GetDetails()
    {
        return _itemBase.GetDetails();
    }

    public abstract void Init(params object[] param);

    public abstract Item SetItemBase(object baseItem);

    public Item Create()
    {
        return _itemBase.Create();
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
            Init();
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
        aux.Add("Cantidad: ", actual + " / "+ itemBase.maxAmount);

        return aux;
    }
}