using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ItemAbstract
{
    [SerializeField]
    string _nameDisplay;

    [SerializeField]
    Sprite _image;

    [Space]
    [SerializeField]
    [TextArea(3, 6)]
    string _details;

    [Range(1, 1000)]
    public int maxAmount = 1;

    public override string nameDisplay => _nameDisplay;

    public override Sprite image => _image;

    protected override Pictionarys<string, string> GetDetails()
    {
        return new Pictionarys<string, string>() { {"Descripcion", _details } };
    }
}

public abstract class ItemAbstract : ScriptableObject
{
    public abstract string nameDisplay { get; }
    public abstract Sprite image { get; }
    public Pictionarys<string, string> details => GetDetails();

    protected abstract Pictionarys<string, string> GetDetails();

    public override string ToString()
    {
        return nameDisplay + "\n\n" + details.ToString("\n", "\n\n");
    }
}


public abstract class Item : ItemAbstract
{
    protected ItemBase _itemBase;

    public override string nameDisplay => _itemBase.nameDisplay;

    public override Sprite image => _itemBase.image;

    protected override Pictionarys<string, string> GetDetails()
    {
        return _itemBase.details;
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

    public override string ToString()
    {
        return _itemBase.ToString();
    }
}

public abstract class Item<T> : Item, Init where T : ItemBase
{
    public T itemBase
    {
        get => (T)_itemBase;
        set
        {
            _itemBase = value;
            Init();
        }
    }

    public abstract void Init();
}

public abstract class ItemStackeable<T> : Item<T> where T : ItemBase
{
    int actual;

    public override int AddAmount(int amount)
    {
        actual += amount;
        int resto = 0;

        if (actual > _itemBase.maxAmount)
        {
            resto = actual - _itemBase.maxAmount;
            actual = _itemBase.maxAmount;
        }

        return resto;
    }

    public override void GetAmounts(out int actual, out int max)
    {
        max = _itemBase.maxAmount;
        actual = this.actual;
    }
}
