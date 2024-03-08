using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(InventoryEntityComponent))]
public class CasterEntityComponent : ComponentOfContainer<Entity>
{
    [SerializeField]
    List<MeleeWeapon> _weapons = new List<MeleeWeapon>();

    [SerializeField]
    EquipedItem<WeaponKata>[] _katas = new EquipedItem<WeaponKata>[3];

    [field: SerializeField]
    public AttackBase flyweight { get; protected set; }

    public DamageContainer additiveDamage;

    public int weaponKataIndex = 0;

    public event System.Action onAttack;

    InventoryEntityComponent inventoryEntity;

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
        inventoryEntity = param.GetInContainer<InventoryEntityComponent>();

        additiveDamage = new DamageContainer(() => flyweight?.additiveDamage);

        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i] = new EquipedItem<WeaponKata>();
            _katas[i].inventoryComponent = inventoryEntity;
        }

        if (flyweight==null)
        {
            flyweight = container.flyweight?.GetFlyWeight<AttackBase>();
            
            if (flyweight?.kataCombos == null)
                return;
        }

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length, 0, 3); i++)
            SetWeaponKataCombo(i);
    }

    public override void OnStayState(Entity param)
    {
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

        actualKata.equiped.Init(inventoryEntity);

        inventoryEntity.inventory.Add(flyweight.kataCombos[index].weapon.Create());

        inventoryEntity.inventory[^1].Init(inventoryEntity);

        actualKata.equiped.ChangeWeapon(inventoryEntity.inventory[^1]);
    }

    public void Init()
    {
        for (int i = 0; i < _katas.Length; i++)
        {
            _katas[i].inventoryComponent = inventoryEntity;

            if (_katas[i].equiped!=null)
            {
                _katas[i].equiped.Init(inventoryEntity);
            }
        }
    }
}

