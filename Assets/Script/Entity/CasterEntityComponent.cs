using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(InventoryEntityComponent))]
public class CasterEntityComponent : ComponentOfContainer<Entity>, ISaveObject
{
    [Tooltip("Armas equipadas y de cambio rapido")]
    public SlotItemList<MeleeWeapon> weapons = new SlotItemList<MeleeWeapon>(5);

    [Tooltip("Ataques especiales que se efectuaran con el combo de ataque")]
    public SlotItemList<WeaponKata> katasCombo = new SlotItemList<WeaponKata>(4);

    [Tooltip("Habilidades equipadas y de cambio rapido")]
    public SlotItemList<AbilityExtCast> abilities = new SlotItemList<AbilityExtCast>(6);

    /*
    [Tooltip("Habilidades que se efectuaran con el combo de habilidades")]
    public SlotItemList<AbilityExtCast> abilitiesCombo = new SlotItemList<AbilityExtCast>(4);
    */

    public DamageContainer additiveDamage;

    public Vector3 aiming;

    public event System.Action onAttack;    

    InventoryEntityComponent inventoryEntity;

    [SerializeField]
    float _energy;

    public int MaxEnergy => flyweight?.energy ?? 0;

    public bool EnergyStatic => flyweight?.energyStatic ?? true;

    public float ChargeEnergy => flyweight?.chargeEnergy ?? 0;

    public float EnergyDefault => flyweight?.energyDefault ?? 0;

    [Tooltip("Representa la parte llena de la barra")]
    public float PositiveEnergy
    {
        get => _energy; 
        set
        {
            _energy = Mathf.Clamp(value, 0, MaxEnergy);
            enabled = true;
        }
    }

    [Tooltip("Representa la parte vacia de la barra")]
    public float NegativeEnergy
    {
        get => MaxEnergy - _energy;
        set
        {
            _energy = Mathf.Clamp(MaxEnergy - value, 0, MaxEnergy);
            enabled = true;
        }
    }

    [field: SerializeField]
    public AttackBase flyweight { get; protected set; }

    public EventControllerMediator abilityControllerMediator { get; set; } = new EventControllerMediator();

    public WeaponKata actualWeapon => weapons.actual.equiped?.defaultKata;

    public AbilityExtCast actualAbility => abilities.actual.equiped;

    public void AttackEvent()
    {
        onAttack?.Invoke();
    }

    void EnergyUpdate()
    {
        if (EnergyStatic || (EnergyDefault * MaxEnergy) == PositiveEnergy)
        {
            enabled = false;
            return;
        }

        float sum = Time.deltaTime * ChargeEnergy;

        if (NegativeEnergy < (1 -EnergyDefault ) * MaxEnergy )
        {
            NegativeEnergy += sum;
        }
        else if (PositiveEnergy < EnergyDefault * MaxEnergy)
        {
            PositiveEnergy += sum;
        }

        if (Mathf.Abs(EnergyDefault * MaxEnergy - PositiveEnergy) <= 0.01f * MaxEnergy)
        {
            PositiveEnergy = EnergyDefault * MaxEnergy;
            enabled = false;
        }
    }

    private void Update()
    {
        EnergyUpdate();
    }

    void SetWeaponKataCombo(int index)
    {
        if (flyweight.kataCombos[index].kata == null)
            return;

        if (katasCombo.Actual(index).equiped != null)
            return;

        var aux = flyweight.kataCombos[index].kata.Create();

        aux.Init(inventoryEntity);

        katasCombo.actual.indexEquipedItem = inventoryEntity.inventory.IndexOf(aux);

        var aux2 = flyweight.kataCombos[index].weapon.Create();

        aux2.Init(inventoryEntity);

        //Debug.Log($"comprobacion : {katasCombo!=null} {katasCombo.actual != null} {katasCombo.actual.equiped != null}");

        katasCombo.actual.equiped.ChangeWeapon(aux2);

        //katasCombo.actual.equiped.onCast += AttackEvent;//ver como mejorar
    }


    public override void OnStayState(Entity param)
    {
        //EnergyUpdate();
        //Debug.Log($"La energia positiva es: {PositiveEnergy} y la negativa es: {NegativeEnergy}");
    }

    public override void OnExitState(Entity param)
    {
        container = null;
        inventoryEntity = null;
        enabled = false;
    }

    public override void OnEnterState(Entity param)
    {
        inventoryEntity = param.GetInContainer<InventoryEntityComponent>();

        weapons.Init(inventoryEntity);
        abilities.Init(inventoryEntity);
        katasCombo.Init(inventoryEntity);
        //abilitiesCombo.Init(inventoryEntity);

        additiveDamage = new DamageContainer(() => flyweight?.additiveDamage);

        if (flyweight == null)
        {
            flyweight = container.flyweight?.GetFlyWeight<AttackBase>();

            if (flyweight?.kataCombos == null)
                return;
        }

        PositiveEnergy = EnergyDefault * MaxEnergy;

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

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
    }
}

