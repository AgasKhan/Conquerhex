using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackEntity : StaticEntity
{
    [SerializeField]
    EquipedItem<WeaponKata>[] katas = new EquipedItem<WeaponKata>[3];

    [field: SerializeField]
    public StructureBase flyweight
    {
        get;
        protected set;
    }

    public int weaponKataIndex = 0;

    public event System.Action onAttack;

    public WeaponKata prin => katas[0].equiped;
    public WeaponKata sec => katas[1].equiped;
    public WeaponKata ter => katas[2].equiped;

    public virtual Damage[] additiveDamage => flyweight.additiveDamage;

    protected override Damage[] vulnerabilities => flyweight.vulnerabilities;

    public EquipedItem<WeaponKata> actualKata
    {
        get
        {
            return katas[weaponKataIndex];
        }
        set
        {
            katas[weaponKataIndex] = value;
        }
    }

    public int EquipedKata(WeaponKata weaponKata)
    {
        foreach (var item in katas)
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
        return katas[index];
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

        for (int i = 0; i < katas.Length; i++)
        {
            katas[i].character = (this);
        }
    }

    void MyStart()
    {
        if (flyweight == null || flyweight.kataCombos == null)
            return;

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length , 0 , 2); i++)
            SetWeaponKataCombo(i);
    }

    void SetWeaponKataCombo(int index)
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