using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : ScriptableObject ,IShowItem
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

    public override string ToString()
    {
        return nameDisplay + "\n\n" + GetDetails().ToString("\n", "\n\n");
    }

    public virtual Pictionarys<string, string> GetDetails()
    {
        return new Pictionarys<string, string>() { { "Descripcion", _details } };
    }
}

public interface IShowItem
{
    public string nameDisplay { get; }
    public Sprite image { get; }
    public Pictionarys<string, string> details => GetDetails();
    public Pictionarys<string, string> GetDetails();
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


public abstract class Item : IShowItem
{
    protected ItemBase _itemBase;

    public string nameDisplay => _itemBase.nameDisplay;

    public Sprite image => _itemBase.image;

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
