using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DinamicEntity, ISwitchState<Character>
{
    [field: SerializeField]
    public BodyBase bodyBase
    {
        get;
        private set;
    }

    [SerializeField]
    EquipedItem<WeaponKata>[] katas = new EquipedItem<WeaponKata>[3];

    public WeaponKata prin => katas[0].equiped;
    public WeaponKata sec => katas[1].equiped;
    public WeaponKata ter => katas[2].equiped;

    public int weaponKataIndex = 0;

    public EquipedItem<WeaponKata> actualKata => katas[weaponKataIndex];

    [SerializeField]
    Detect<RecolectableItem> areaFarming;

    public Damage[] additiveDamage => bodyBase.additiveDamage;

    protected override Damage[] vulnerabilities => bodyBase.vulnerabilities;

    AudioManager audioM;

    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if(_ia!=null)
            {
                _ia.OnExitState(this);
            }
            else if(value!=null)
            {
                MyUpdates += IAUpdate;
            }

            _ia = value;
            _ia.OnEnterState(this);
        }
    }

    [SerializeField]
    IState<Character> _ia;

    public void SetWeaponKataCombo(int index, WeaponKataCombo combo)
    {
        if (combo.kata == null)
            return;

        weaponKataIndex = index;

        inventory.Add(combo.kata.Create());

        actualKata.indexEquipedItem = inventory.Count-1;

        inventory.Add(combo.weapon.Create());

        actualKata.equiped.Init(this, inventory.Count-1);
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


    public override void TakeDamage(Damage dmg)
    {
        var vulDmg = bodyBase.vulnerabilities;

        for (int i = 0; i < vulDmg.Length; i++)
        {
            if (dmg.typeInstance == vulDmg[i].typeInstance)
            {
                dmg.amount *= vulDmg[i].amount;
            }
        }

        base.TakeDamage(dmg);
    }


    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
        MyStarts += MyStart;
    }

    void MyAwake()
    {
        health.Init(bodyBase.life, bodyBase.regen);

        areaFarming.radius = bodyBase.areaFarming;

        weightCapacity = bodyBase.weightCapacity;

        audioM = GetComponent<AudioManager>();

        for (int i = 0; i < katas.Length; i++)
        {
            katas[i].character = (this);
        }

        SetWeaponKataCombo(0, bodyBase.principal);

        SetWeaponKataCombo(1, bodyBase.secondary);

        SetWeaponKataCombo(2, bodyBase.tertiary);

        _ia = GetComponent<IState<Character>>();
    }

    void MyStart()
    {
        if (_ia != null)
        {
            _ia.OnEnterState(this);
            MyUpdates += IAUpdate;
        }
    }

    void MyUpdate()
    {
        var recolectables = areaFarming.Area(transform.position, (algo) => { return true; });
        foreach (var recolectable in recolectables)
        {

            //if (currentWeight + recolectable.weight <= weightCapacity)
            {
                recolectable.Recolect(this);
            }
        }
    }

    void IAUpdate()
    {
        _ia.OnStayState(this);
    }
}

[System.Serializable]
public class EquipedItem<T> where T : Item
{
    [HideInInspector]
    public Character character;

    public T equiped
    {
        get
        {
            if (indexEquipedItem < 0 || character ==null  || indexEquipedItem >= character.inventory.Count || !(character.inventory[indexEquipedItem] is T))
                return default;
            else
                return (T)character.inventory[indexEquipedItem];
        }
    }

    public int indexEquipedItem = -1;
}
