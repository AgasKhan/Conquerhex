using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ShowDetails
{
    [Range(1, 1000)]
    public int maxAmount = 1;

    protected System.Type _itemType;
    protected abstract void SetCreateItemType();
    
    public Item Create() 
    {
        var aux = System.Activator.CreateInstance(_itemType) as Item;

        aux.SetItemBase(this);

        return aux;
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        SetCreateItemType();
        MyEnable();
    }

    private void OnDisable()
    {
        MyDisable();
    }

    private void OnDestroy()
    {
        MyDisable();
    }

    protected virtual void MyDisable()
    {
        Manager<ItemBase>.pic.Remove(nameDisplay);
    }

    protected virtual void MyEnable()
    {
        Manager<ItemBase>.pic.Add(nameDisplay, this);
    }
}






[System.Serializable]
public abstract class Item : IShowDetails, Init
{
    [SerializeField]
    private ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public void SetItemBase(ItemBase item)
    {
        _itemBase = item;
        Init();
    }

    public abstract void Init(params object[] param);


    public virtual Pictionarys<string, string> GetDetails()
    {
        return _itemBase.GetDetails();
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
    public virtual int AddAmount(int amount)
    {
        return amount;
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

public abstract class Item<T> : Item where T : ItemBase
{
    public T itemBase
    {
        get => (T)GetItemBase();
        set => SetItemBase(value);
    }
}

public abstract class ItemStackeable<T> : Item<T> where T : ItemBase
{
    int actual = 1;

    public override int AddAmount(int amount)
    {
        actual += amount;
        int resto = 0;

        if (actual > itemBase.maxAmount)
        {
            resto = actual - itemBase.maxAmount;
            actual = itemBase.maxAmount;
        }
        else if (actual <= 0)
        {
            resto = actual;
        }

        return resto;
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