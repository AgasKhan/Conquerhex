using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(InventoryEntityComponent))]
public class CasterEntityComponent : ComponentOfContainer<Entity>
{
    [Tooltip("Armas equipadas y de cambio rapido")]
    public SlotItemList<MeleeWeapon> weapons = new SlotItemList<MeleeWeapon>(5);

    [Tooltip("Habilidaddes equipadas y de cambio rapido")]
    public SlotItemList<MeleeWeapon> abilities = new SlotItemList<MeleeWeapon>(5);

    [Tooltip("Ataques especiales que se efectuaran con el combo de ataque")]
    public SlotItemList<WeaponKata> katasCombo = new SlotItemList<WeaponKata>(4);

    [Tooltip("Habilidades que se efectuaran con el combo de habilidades")]
    public SlotItemList<WeaponKata> abilitiesCombo = new SlotItemList<WeaponKata>(4);

    public DamageContainer additiveDamage;

    public Vector3 aiming;

    public event System.Action onAttack;

    public event System.Action OnActionEnter;

    public event System.Action OnActionExit;

    FSMAutomaticEnd<CasterEntityComponent> internalFsm = new FSMAutomaticEnd<CasterEntityComponent>();

    InventoryEntityComponent inventoryEntity;

    [field: SerializeField]
    public AttackBase flyweight { get; protected set; }

    public EventControllerMediator attack { get; set; } = new EventControllerMediator();

    public EventControllerMediator ability { get; set; } = new EventControllerMediator();

    public WeaponKata actualWeapon => weapons.actual.equiped.defaultKata;

    public WeaponKata actualAbility => abilities.actual.equiped.defaultKata;

    public bool end => internalFsm.end;

    public void AttackEvent()
    {
        onAttack?.Invoke();
    }

    public void Attack(int number)
    {
        WeaponKata weaponKata;

        if (number==0)
        {
            weaponKata = actualWeapon;
        }
        else
        {
            weaponKata = katasCombo.Actual(number - 1).equiped;
        }

        EnterState(weaponKata);
    }

    public void Ability(int number)
    {
        WeaponKata weaponKata;

        if (number == 0)
        {
            weaponKata = actualAbility;
        }
        else
        {
            weaponKata = abilitiesCombo.Actual(number - 1).equiped;
        }

        EnterState(weaponKata);
    }

    void SetWeaponKataCombo(int index)
    {
        if (flyweight.kataCombos[index].kata == null)
            return;

        if (katasCombo.Actual(index).equiped != null)
            return;

        inventoryEntity.inventory.Add(flyweight.kataCombos[index].kata.Create());

        katasCombo.actual.indexEquipedItem = inventoryEntity.inventory.Count - 1;

        inventoryEntity.inventory[^1].Init(inventoryEntity);

        inventoryEntity.inventory.Add(flyweight.kataCombos[index].weapon.Create());

        inventoryEntity.inventory[^1].Init(inventoryEntity);

        //Debug.Log($"comprobacion : {katasCombo!=null} {katasCombo.actual != null} {katasCombo.actual.equiped != null}");

        katasCombo.actual.equiped.ChangeWeapon(inventoryEntity.inventory[^1]);
    }

    void EnterState(WeaponKata weaponKata)
    {
        internalFsm.EnterState(weaponKata);
    }

    void TriggerEnter()
    {
        OnActionEnter?.Invoke();
    }

    void TriggerExit()
    {
        OnActionExit?.Invoke();
    }

    public override void OnStayState(Entity param)
    {
        internalFsm.UpdateState();
    }

    public override void OnExitState(Entity param)
    {
        container = null;
        inventoryEntity = null;
    }

    public override void OnEnterState(Entity param)
    {
        inventoryEntity = param.GetInContainer<InventoryEntityComponent>();

        weapons.Init(inventoryEntity);
        abilities.Init(inventoryEntity);
        katasCombo.Init(inventoryEntity);
        abilitiesCombo.Init(inventoryEntity);

        additiveDamage = new DamageContainer(() => flyweight?.additiveDamage);

        if (flyweight == null)
        {
            flyweight = container.flyweight?.GetFlyWeight<AttackBase>();

            if (flyweight?.kataCombos == null)
                return;
        }

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length, 0, 3); i++)
            SetWeaponKataCombo(i);
    }

    public void Init()
    {
        for (int i = 0; i < katasCombo.Count; i++)
        {
            katasCombo[i].inventoryComponent = inventoryEntity;

            if (katasCombo[i].equiped != null)
            {
                katasCombo[i].equiped.Init(inventoryEntity);
            }
        }
    }

    private void Awake()
    {
        internalFsm.Init(this);

        internalFsm.onEnter += fsm => TriggerEnter();

        internalFsm.onExit += fsm => TriggerExit();
    }
}
