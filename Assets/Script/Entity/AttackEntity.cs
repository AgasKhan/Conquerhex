using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackEntity : StaticEntity, Init
{
    [SerializeField]
    EquipedItem<WeaponKata>[] _katas = new EquipedItem<WeaponKata>[3];

    [field: SerializeField]
    public StructureBase flyweight
    {
        get;
        protected set;
    }

    public int weaponKataIndex = 0;

    public event System.Action onAttack;

    public virtual Damage[] additiveDamage => flyweight.additiveDamage;

    protected override Damage[] vulnerabilities => flyweight.vulnerabilities;

    public EquipedItem<WeaponKata> actualKata
    {
        get
        {
            return _katas[weaponKataIndex];
        }
        set
        {
            _katas[weaponKataIndex] = value;
        }
    }

    public int EquipedKata(WeaponKata weaponKata)
    {
        foreach (var item in _katas)
        {
            if (weaponKata == item.equiped)
            {
                return item.indexEquipedItem;
            }
        }

        return -1;
    }

    public EquipedItem<WeaponKata> ActualKata(int index)
    {
        return _katas[index];
    }

    public void AttackEvent()
    {
        onAttack?.Invoke();
    }

    public override void TakeDamage(ref Damage dmg)
    {
        var vulDmg = flyweight.vulnerabilities;

        for (int i = 0; i < vulDmg.Length; i++)
        {
            if (dmg.typeInstance == vulDmg[i].typeInstance)
            {
                dmg.amount *= vulDmg[i].amount;
            }
        }

        base.TakeDamage(ref dmg);
    }

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
        MyStarts += MyStart;
    }

    void MyAwake()
    {
        if(flyweight!=null)
            health.Init(flyweight.life, flyweight.regen);

        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i] = new EquipedItem<WeaponKata>();
            _katas[i].character = this;
        }
    }

    void MyStart()
    {
        if (flyweight == null || flyweight.kataCombos == null)
            return;

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length , 0 , 3); i++)
            SetWeaponKataCombo(i);
    }

    protected void SetWeaponKataCombo(int index)
    {
        if (flyweight.kataCombos[index].kata == null)
            return;

        weaponKataIndex = index;

        if (actualKata.equiped != null)
            return;

        inventory.Add(flyweight.kataCombos[index].kata.Create());

        actualKata.indexEquipedItem = inventory.Count - 1;

        inventory.Add(flyweight.kataCombos[index].weapon.Create());

        actualKata.equiped.Init(this, inventory.Count - 1);
    }

    public void Init(params object[] param)
    {
        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i].character = this;

            if (_katas[i].equiped!=null)
            {
                _katas[i].equiped.Init(this, _katas[i].equiped.indexWeapon);
            }
        }
    }
}

[System.Serializable]
public class EquipedItem<T> where T : Item
{
    [System.NonSerialized]
    public AttackEntity character;

    public event System.Action<int, T> toChange;

    public T equiped
    {
        get
        {
            if (indexEquipedItem < 0 || character == null || indexEquipedItem >= character.inventory.Count || !(character.inventory[indexEquipedItem] is T))
                return default;
            else
                return (T)character.inventory[indexEquipedItem];
        }
    }

    [SerializeField]
    int _indexEquipedItem = -1;

    public int indexEquipedItem
    {
        get => _indexEquipedItem;
        set
        {
            _indexEquipedItem = value;
            toChange?.Invoke(_indexEquipedItem, equiped);
        }
    }
}