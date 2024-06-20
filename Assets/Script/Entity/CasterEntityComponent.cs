using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(InventoryEntityComponent))]
public class CasterEntityComponent : ComponentOfContainer<Entity>, ISaveObject, IDamageable
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

    public Vector3 Aiming
    {
        get => _aiming;
        set
        {
            _aiming = value;
            onAiming?.Invoke(value);
        }
    }

    Vector3 _aiming;

    public event System.Action<Ability> onCast;
    public event System.Action<Ability> onPreCast;
    public event System.Action<Vector3> onAiming;
    public event System.Action<(Damage dmg, int weightAction, Vector3? origin)> onTakeDamage;

    public event System.Action<(float, float, float)> energyUpdate;
    public event System.Action<float> leftEnergyUpdate;
    public event System.Action<float> rightEnergyUpdate;

    InventoryEntityComponent inventoryEntity;

    [SerializeField,Range(-100,100), Tooltip(   "en caso de ser calor (positivo): es el porcentage de cuanta mas energia ganara con frio y cuanta menos energia perdera con calor" +
                                "\nen caso de ser frio (negativo): es el porcentage de cuanta menos energia ganara con frio y cuanta mas energia perdera con calor")]
    float _buffEnergy;

    [SerializeField]
    float _energy;

    float energy
    {
        get => _energy;
        set
        {
            float f = Mathf.Clamp(value, 0, MaxEnergy);
            energyUpdate?.Invoke((f / MaxEnergy, _energy - f, f));
            _energy = f;
        }
    }

    public float buffEnergy
    {
        get => _buffEnergy/100;
        set
        {
            _buffEnergy = Mathf.Clamp(value, -100, 100);
        }
    }

    public int MaxEnergy => flyweight?.energy ?? 0;

    public bool EnergyStatic => flyweight?.energyStatic ?? true;

    public float ChargeEnergy => flyweight?.chargeEnergy ?? 0;

    public float EnergyDefault => flyweight?.energyDefault ?? 0;


    [Tooltip("Representa la parte llena de la barra")]
    public float positiveEnergy
    {
        get => energy;
        set
        {
            energy = value;
            enabled = true;
        }
    }

    [Tooltip("Representa la parte vacia de la barra")]
    public float negativeEnergy
    {
        get => MaxEnergy - energy;
        set
        {
            energy = MaxEnergy - value;
            enabled = true;
        }
    }

    [field: SerializeField]
    public AttackBase flyweight { get; protected set; }

    public EventControllerMediator abilityControllerMediator { get; set; } = new EventControllerMediator();

    public WeaponKata actualWeapon => weapons.actual.equiped?.defaultKata;

    public AbilityExtCast actualAbility => abilities.actual.equiped;

    public void CastEvent(Ability ability)
    {
        onCast?.Invoke(ability);
    }

    public void PreCastEvent(Ability ability)
    {
        onPreCast?.Invoke(ability);
    }
    
    /// <summary>
    /// Genera un consumo positivo de energia (pierdo energia)
    /// </summary>
    /// <param name="energy"></param>
    /// <returns>Falso en caso de q no se pueda realizar el consumo</returns>
    public bool PositiveEnergy(float energy)
    {
        float calc = energy * (buffEnergy < 0 ? (1 + Mathf.Abs(buffEnergy)) : 1 - Mathf.Abs(buffEnergy));

        if (calc > positiveEnergy)
        {
            leftEnergyUpdate?.Invoke(calc / MaxEnergy);
            return false;
        }   

        positiveEnergy -= calc;

        return true;
    }


    /// <summary>
    /// Genero un consumo negativo de energia (gano energia)
    /// </summary>
    /// <param name="energy"></param>
    /// <returns>Falso en caso de q no se pueda realizar la ganancia</returns>
    public bool NegativeEnergy(float energy)
    {
        float calc = energy * (buffEnergy > 0 ? (1 + Mathf.Abs(buffEnergy)) : 1 - Mathf.Abs(buffEnergy));

        if (calc > negativeEnergy)
        {
            rightEnergyUpdate?.Invoke(calc / MaxEnergy);
            return false;
        }

        negativeEnergy -= calc;

        return true;
    }
    
    void EnergyUpdate()
    {
        if (EnergyStatic || (EnergyDefault * MaxEnergy) == positiveEnergy)
        {
            enabled = false;
            return;
        }

        float sum = Time.deltaTime * ChargeEnergy;

        if (negativeEnergy < (1 -EnergyDefault ) * MaxEnergy )
        {
            negativeEnergy += sum;
        }
        else if (positiveEnergy < EnergyDefault * MaxEnergy)
        {
            positiveEnergy += sum;
        }

        if (Mathf.Abs(EnergyDefault * MaxEnergy - positiveEnergy) <= 0.01f * MaxEnergy)
        {
            _energy = EnergyDefault * MaxEnergy;
            energyUpdate?.Invoke((_energy / MaxEnergy,0,_energy));
            enabled = false;
        }
    }

    private void Update()
    {
        EnergyUpdate();
    }

    void SetWeaponKataCombo(int index)
    {
        var indexToEquip = flyweight.kataCombos[index].indexToEquip == -1 ? index : flyweight.kataCombos[index].indexToEquip;

        if (flyweight.kataCombos[index].kata == null || indexToEquip > katasCombo.Count || katasCombo.Actual(indexToEquip).equiped != null)
            return;

        var aux = flyweight.kataCombos[index].kata.Create();

        aux.Init(inventoryEntity);

        var aux2 = flyweight.kataCombos[index].weapon.Create();

        aux2.Init(inventoryEntity);

        ((WeaponKata)aux).isDefault = flyweight.kataCombos[index].isDefault;
        ((MeleeWeapon)aux2).isDefault = flyweight.kataCombos[index].isDefault;

        ((WeaponKata)aux).CreateCopy(out int otherindex);

        katasCombo.actual.isModifiable = flyweight.kataCombos[index].isModifiable;

        katasCombo.actual.indexEquipedItem = otherindex;

        //Debug.Log($"comprobacion : {katasCombo!=null} {katasCombo.actual != null} {katasCombo.actual.equiped != null}");

        katasCombo.actual.equiped.ChangeWeapon(aux2);
    }

    public void SetAbility(AbilityToEquip abilityToEquip)
    {
        if (abilityToEquip == null || abilityToEquip.indexToEquip > abilities.Count || abilities.Actual(abilityToEquip.indexToEquip).equiped != null)
            return;

        var aux = abilityToEquip.ability.Create();

        aux.Init(inventoryEntity);

        ((AbilityExtCast)aux).isDefault = abilityToEquip.isDefault;

        ((AbilityExtCast)aux).CreateCopy(out int otherindex);

        abilities.actual.isModifiable = abilityToEquip.isModifiable;

        abilities.actual.indexEquipedItem = otherindex;
    }

    void SetAbility(int index)
    {
        var indexToEquip = flyweight.abilities[index].indexToEquip == -1 ? index : flyweight.abilities[index].indexToEquip;

        if (flyweight.abilities[index] == null || indexToEquip > abilities.Count || abilities.Actual(indexToEquip).equiped != null)
            return;

        var aux = flyweight.abilities[index].ability.Create();

        aux.Init(inventoryEntity);

        ((AbilityExtCast)aux).isDefault = flyweight.abilities[index].isDefault;

        ((AbilityExtCast)aux).CreateCopy(out int otherindex);

        abilities.actual.isModifiable = flyweight.abilities[index].isModifiable;

        abilities.actual.indexEquipedItem = otherindex;
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

        positiveEnergy = EnergyDefault * MaxEnergy;

        for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length, 0, 3); i++)
        {
            SetWeaponKataCombo(i);
        }
            
        for (int i = 0; i < Mathf.Clamp(flyweight.abilities.Length, 0, 3); i++)
        {
            SetAbility(i);
        }
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

    public void InternalTakeDamage(ref Damage dmg, int weightAction = 0, Vector3? damageOrigin = null)
    {
        onTakeDamage?.Invoke((dmg, weightAction, damageOrigin));
    }
}

