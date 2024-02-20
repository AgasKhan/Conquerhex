using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(InventoryEntityComponent))]
public class AttackEntityComponent : ComponentOfContainer<Entity>
{
    [SerializeField]
    EquipedItem<WeaponKata>[] _katas = new EquipedItem<WeaponKata>[3];

    [field: SerializeField]
    public StructureBase flyweight { get; protected set; }
    public Damage[] additiveDamage => flyweight.additiveDamage;

    public int weaponKataIndex = 0;

    public event System.Action onAttack;

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

    InventoryEntityComponent inventoryEntity;

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

    public override void OnEnterState(Entity param)
    {
        container = param;
        inventoryEntity = param.GetInContainer<InventoryEntityComponent>();

        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i] = new EquipedItem<WeaponKata>();
            _katas[i].inventoryComponent = inventoryEntity;
        }

        if (flyweight == null || flyweight.kataCombos == null)
            return;

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length, 0, 3); i++)
            SetWeaponKataCombo(i);
    }

    public override void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
    }

    public override void OnExitState(Entity param)
    {
        container = null;
        inventoryEntity = null;
    }


    protected void SetWeaponKataCombo(int index)
    {
        if (flyweight.kataCombos[index].kata == null)
            return;

        weaponKataIndex = index;

        if (actualKata.equiped != null)
            return;

        inventoryEntity.inventory.Add(flyweight.kataCombos[index].kata.Create());

        actualKata.indexEquipedItem = inventoryEntity.inventory.Count - 1;

        inventoryEntity.inventory.Add(flyweight.kataCombos[index].weapon.Create());

        actualKata.equiped.Init(this, inventoryEntity.inventory.Count - 1);
    }

    public void Init()
    {
        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i].inventoryComponent = inventoryEntity;

            if (_katas[i].equiped!=null)
            {
                _katas[i].equiped.Init(this, _katas[i].equiped.indexWeapon);
            }
        }
    }
}

