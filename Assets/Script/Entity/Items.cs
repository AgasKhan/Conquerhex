using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string nameDisplay;
    public Sprite image;

    [Space]
    [TextArea(3, 6)]
    public string description;

    [Range(1, 1000)]
    public int maxAmount = 1;

    public override string ToString()
    {
        return nameDisplay + "\n" + description;
    }
}

public abstract class Item
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
        return new List<string>() { _itemBase.description };
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
