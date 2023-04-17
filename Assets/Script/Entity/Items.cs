using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject, IShowItem
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

    public string nameDisplay => _nameDisplay;

    public Sprite image => _image;

    public Pictionarys<string,string> details => GetDetails();

    protected virtual Pictionarys<string, string> GetDetails()
    {
        return new Pictionarys<string, string>() { {"Descripcion", _details } };
    }

    public override string ToString()
    {
        return nameDisplay + "\n" + details.ToString("\n", "\n\n");
    }
}

public interface IShowItem
{
    public string nameDisplay { get; }
    public Sprite image { get;  }
    public Pictionarys<string, string> details { get; }
}

public abstract class Item : IShowItem
{
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public Pictionarys<string, string> details => GetDetails();

    protected virtual Pictionarys<string, string> GetDetails()
    {
        return _itemBase.details;
    }

    public virtual void GetAmounts(out int actual, out int max)
    {
        max = _itemBase.maxAmount;
        actual = 1;
    }

    public virtual int AddAmount(int amount)
    {
        return _itemBase.maxAmount;
    }

    public override string ToString()
    {
        return _itemBase.ToString();
    }
}

public abstract class Item<T> : Item where T : ItemBase
{
    public T itemBase => (T)_itemBase;
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
