using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject, IShowItem
{
    public string nameDisplay;
    public Sprite image;

    [Space]
    [TextArea(3, 6)]
    public string details;

    [Range(1, 1000)]
    public int maxAmount = 1;

    string IShowItem.nameDisplay => nameDisplay;

    Sprite IShowItem.image => image;

    List<string> IShowItem.details => new List<string>() { details };

    public override string ToString()
    {
        return nameDisplay + "\n" + details;
    }
}

public interface IShowItem
{
    public string nameDisplay { get; }
    public Sprite image { get;  }
    public List<string> details { get; }
}

public abstract class Item : IShowItem
{
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

    public List<string> details => GetDetails();

    public virtual void GetAmounts(out int actual, out int max)
    {
        max = _itemBase.maxAmount;
        actual = 1;
    }

    public virtual int AddAmount(int amount)
    {
        return _itemBase.maxAmount;
    }

    protected virtual List<string> GetDetails()
    {
        return new List<string>() { _itemBase.details };
    }

    public override string ToString()
    {
        return nameDisplay + "\n" + string.Join("\n", details);
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
