using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

[RequireComponent(typeof(AimingEntityComponent))]
[RequireComponent(typeof(InventoryEntityComponent))]
public class CasterEntityComponent : ComponentOfContainer<Entity>, ISaveObject, IDamageable
{
    [Tooltip("Armas equipadas y de cambio rapido")]
    public SlotItemList<MeleeWeapon> weapons = new SlotItemList<MeleeWeapon>(5);

    [Tooltip("Ataques especiales que se efectuaran con el combo de ataque")]
    public SlotItemList<WeaponKata> katas = new SlotItemList<WeaponKata>(4);

    [Tooltip("Habilidades equipadas y de cambio rapido")]
    public SlotItemList<AbilityExtCast> abilities = new SlotItemList<AbilityExtCast>(6);

    [Tooltip("Combos de ataque")]
    public SlotItemList<Ability> combos = new SlotItemList<Ability>(15);

    public DamageContainer additiveDamage;

    public event System.Action<Ability> onApplyCast;
    
    public event System.Action<AnimationInfo.Data> onAnimation;

    public event System.Action<(Damage dmg, int weightAction, Vector3? origin)> onTakeDamage;

    /// <summary>
    /// percentage, diference, energy
    /// </summary>
    public event System.Action<(float, float, float)> energyUpdate;
    public event System.Action<float> leftEnergyUpdate;
    public event System.Action<float> rightEnergyUpdate;
    
    public event System.Action<int, MeleeWeapon> onEquipInSlotWeapon; 

    InventoryEntityComponent inventoryEntity;

    AimingEntityComponent aimingSystem;

    Ability _abilityCasting;

    FSMAutomaticEnd<Character> param;

    System.Action<Ability> _OnEnter;

    System.Action<Ability> _OnExit;

    [SerializeField,Range(-100,100), Tooltip(   "en caso de ser calor (positivo): es el porcentage de cuanta mas energia ganara con frio y cuanta menos energia perdera con calor" +
                                "\nen caso de ser frio (negativo): es el porcentage de cuanta menos energia ganara con frio y cuanta mas energia perdera con calor")]
    float _buffEnergy;

    [SerializeField]
    float _energy;

    [field: SerializeField]
    bool activeHasCoolDown = true;

    bool _hasCooldown = true;


    [field: SerializeField]
    bool activeHasEnergyConsuption = true;

    bool _hasEnergyConsuption = true;

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

    public bool End => abilityCasting?.End ?? true;

    public bool HasCooldown
    {
        get => _hasCooldown;
        private set
        {
            if(activeHasCoolDown)
            {
                _hasCooldown = value;
            }
        }
    }




    public bool HasEnergyConsuption
    { 
        get => _hasEnergyConsuption; 
        private set
        {
            if(activeHasEnergyConsuption)
            {
                _hasEnergyConsuption = value;
            }
        }
    }





    public event System.Action<Ability> onEnterCasting;

    public event System.Action<Ability> onExitCasting;

    /// <summary>
    /// Evento que se ejecuta al finalizar el casteo <br/>
    /// No debe de ser utilizado para realizar transiciones
    /// </summary>
    event System.Action OnExitCastingOnlyOne
    {
        add
        {
            System.Action<Ability> action = null;

            action = (a) =>
            {
                value();
                //_OnExit -= value;
                _OnExit -= action;
            };

            _OnExit += action;
        }
        remove
        {
            Debug.LogError("No podes desuscribirte manualmente de el evento de desuscripcion automatica");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Ability abilityCasting
    {
        get => _abilityCasting;
        set => SwitchAbilty(value);
    }

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
        onApplyCast?.Invoke(ability);
    }

    public void OnAnimation(AnimationInfo.Data data)
    {
        onAnimation?.Invoke(data);
    }

    public void OnEnter(Ability ability)
    {
        onEnterCasting?.Invoke(ability);
    }

    public void OnExit(Ability ability)
    {
        onExitCasting?.Invoke(ability);
    }

    /// <summary>
    /// Compruebo si se puede realizar un consumo de cualquier tipo
    /// </summary>
    /// <param name="energy">la energia que se desea testear, puede ser negativa o positiva</param>
    /// <param name="energyBuff">la energia despues de aplicarle los buffos</param>
    /// <param name="percentageRequire">el porcentage representativo de cuanta energia seria en base a la barra</param>
    /// <returns>Verdadero en caso que se pueda realizar la operacion</returns>
    public bool EnergyComprobation(float energy, out float energyBuff, out float percentageRequire)
    {
        if(energy>0)
        {
            energyBuff = energy * (buffEnergy < 0 ? (1 + Mathf.Abs(buffEnergy)) : 1 - Mathf.Abs(buffEnergy));

            percentageRequire = energyBuff / MaxEnergy;

            return energyBuff <= positiveEnergy;
        }
        else
        {
            energy = -energy;

            energyBuff = energy * (buffEnergy > 0 ? (1 + Mathf.Abs(buffEnergy)) : 1 - Mathf.Abs(buffEnergy));

            percentageRequire = energyBuff / MaxEnergy;

            return energyBuff <= negativeEnergy;
        }
    }
    
    /// <summary>
    /// Genera un consumo positivo de energia (pierdo energia)
    /// </summary>
    /// <param name="energy"></param>
    /// <returns>Falso en caso de q no se pueda realizar el consumo</returns>
    public bool PositiveEnergy(float energy)
    {
        if (!EnergyComprobation(energy,out float calc, out float percentage))
        {
            leftEnergyUpdate?.Invoke(percentage);
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
        if (!EnergyComprobation(-energy, out float calc, out float percentage))
        {
            rightEnergyUpdate?.Invoke(percentage);
            return false;
        }

        negativeEnergy -= calc;

        return true;
    }


    public void Attack(int i, Vector2 dir, System.Action onAbilityEnter = null, System.Action onAbilityExit = null)
    {
        HasCooldown = true;
        HasEnergyConsuption = true;

        Ability ability;

        if (i == 0)
        {
            ability = actualWeapon;
        }
        else
        {
            ability = katas.Actual(i - 1).equiped;
        }

        SwitchAbilty(ability, onAbilityEnter, onAbilityExit);

        abilityCasting?.ControllerDown(dir, 0);
    }

    public void Ability(int i, Vector2 dir, System.Action onAbilityEnter = null, System.Action onAbilityExit = null)
    {
        HasCooldown = true;
        HasEnergyConsuption = true;

        if (i != 0)
        {
            i += 1;
        }

        SwitchAbilty(abilities.Actual(i).equiped, onAbilityEnter, onAbilityExit);

        abilityCasting?.ControllerDown(dir,0);
    }

    public void AlternateAbility(Vector2 dir, System.Action onAbilityEnter = null, System.Action onAbilityExit = null)
    {
        HasCooldown = true;
        HasEnergyConsuption = true;

        SwitchAbilty(abilities.Actual(1).equiped, onAbilityEnter, onAbilityExit);

        abilityCasting?.ControllerDown(dir,0);
    }

    public void ComboAttack(int i, System.Action onAbilityEnter = null, System.Action onAbilityExit = null)
    {
        HasCooldown = false;

        SwitchAbilty(combos.Actual(i).equiped, onAbilityEnter, onAbilityExit);

        abilityCasting?.ControllerDown(abilityControllerMediator.dir, 0);
    }

    /// <summary>
    /// Se encarga de realizar los cambios de habilidad
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="onAbilityEnter"></param>
    /// <param name="onAbilityExit"></param>
    void SwitchAbilty(Ability ability, System.Action onAbilityEnter = null, System.Action onAbilityExit = null)
    {
        if (_abilityCasting != null)
        {
            abilityCasting.OnExitState(this);
            abilityControllerMediator -= _abilityCasting;
            _OnExit.Invoke(abilityCasting);

            _abilityCasting = ability;

            if (ability == null)
                return;

            if(onAbilityExit!=null)
                OnExitCastingOnlyOne += onAbilityExit;
            
            abilityCasting?.OnEnterState(this);
            abilityControllerMediator += abilityCasting;
            _OnEnter?.Invoke(abilityCasting);
            onAbilityEnter?.Invoke();
        }
        else if (ability != null)
        {
            _abilityCasting = ability;

            if (onAbilityExit != null)
                OnExitCastingOnlyOne += onAbilityExit;

            _abilityCasting.OnEnterState(this);
            abilityControllerMediator += _abilityCasting;
            _OnEnter?.Invoke(abilityCasting);
            onAbilityEnter?.Invoke();
        }
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

    void SetWeapon(int index)
    {
        //Cambios: Ahora los slots se configuran antes que los items que poseen y se agrego el seteo de la nueva variable "isBlocked"
        weapons.Actual(0).isBlocked = flyweight.weaponToEquip.isBlocked;
        //Fin de cambios

        if (flyweight.weaponToEquip?.weapon == null /*|| indexToEquip > katas.Count */ || weapons.actual.equiped != null)//Se agregó una consideración nueva en la que se pregunta si el item a equipar es null
            return;

        var aux2 = (MeleeWeapon)flyweight.weaponToEquip.weapon.Create();
        int indexInventory = aux2.Init(inventoryEntity);
        aux2.isDefault = flyweight.weaponToEquip.isDefault;
        weapons.actual.indexEquipedItem = indexInventory;
    }

    void SetWeaponKata(int index)
    {
        var indexToEquip = flyweight.kataCombos[index].indexToEquip == -1 ? index : flyweight.kataCombos[index].indexToEquip;

        //Cambios: Ahora los slots se configuran antes que los items que poseen y se agrego el seteo de la nueva variable "isBlocked"
        katas.Actual(indexToEquip).isModifiable = flyweight.kataCombos[index].isModifiable;
        katas.actual.isBlocked = flyweight.kataCombos[index].isBlocked;
        //Fin de cambios

        if (flyweight.kataCombos[index]?.kata == null || indexToEquip > katas.Count || katas.actual.equiped != null)//Se agregó una consideración nueva en la que se pregunta si el item a equipar es null
            return;

        var aux = flyweight.kataCombos[index].kata.Create();

        aux.Init(inventoryEntity);

        var aux2 = flyweight.kataCombos[index].weapon.Create();

        aux2.Init(inventoryEntity);

        ((WeaponKata)aux).isDefault = flyweight.kataCombos[index].isDefault;
        ((MeleeWeapon)aux2).isDefault = flyweight.kataCombos[index].isDefault;

        if (flyweight.kataCombos[index].isDefault)//Se agrega la condicion para setear el item por default solo cuando corresponde
            katas.actual.SetDefaultItem((WeaponKata)aux);

        ((WeaponKata)aux).CreateCopy(out int otherindex);

        katas.actual.indexEquipedItem = otherindex;

        //Debug.Log($"comprobacion : {katasCombo!=null} {katasCombo.actual != null} {katasCombo.actual.equiped != null}");

        katas.actual.equiped.ChangeWeapon(aux2);
    }


    void SetAbility(int index)
    {
        var indexToEquip = flyweight.abilities[index].indexToEquip == -1 ? index : flyweight.abilities[index].indexToEquip;

        //Cambios: Ahora los slots se configuran antes que los items que poseen y se agrego el seteo de la nueva variable "isBlocked"
        abilities.Actual(indexToEquip).isModifiable = flyweight.abilities[index].isModifiable;
        abilities.actual.isBlocked = flyweight.abilities[index].isBlocked;
        //Fin de cambios

        if (flyweight.abilities[index]?.ability == null || indexToEquip > abilities.Count || abilities.actual.equiped != null)//Se agregó una consideración nueva en la que se pregunta si el item a equipar es null
            return;

        var aux = flyweight.abilities[index].ability.Create();

        aux.Init(inventoryEntity);

        ((AbilityExtCast)aux).isDefault = flyweight.abilities[index].isDefault;

        if (flyweight.abilities[index].isDefault)//Se agrega la condicion para setear el item por default solo cuando corresponde
            abilities.actual.SetDefaultItem((AbilityExtCast)aux);

        ((AbilityExtCast)aux).CreateCopy(out int otherindex);

        abilities.actual.indexEquipedItem = otherindex;
    }

    void SetCombo(int index)
    {
        var indexToEquip = flyweight.combos[index].indexToEquip == -1 ? index : flyweight.combos[index].indexToEquip;

        //Cambios: Ahora los slots se configuran antes que los items que poseen y se agrego el seteo de la nueva variable "isBlocked"
        Debug.Log("INDEX OF Combo "+ index + " : " + indexToEquip);
        combos.Actual(indexToEquip).isModifiable = flyweight.combos[index].isModifiable;
        combos.actual.isBlocked = flyweight.combos[index].isBlocked;
        //Fin de cambios

        if (flyweight.combos[index]?.ability == null || indexToEquip > combos.Count || combos.actual.equiped != null)//Se agregó una consideración nueva en la que se pregunta si el item a equipar es null
            return;

        Ability aux = (Ability)flyweight.combos[index].ability.Create();

        aux.Init(inventoryEntity);

        aux.isDefault = flyweight.combos[index].isDefault;

        aux = aux.CreateCopy(out int otherindex);

        combos.actual.indexEquipedItem = otherindex;
        //


        if (aux is WeaponKata weaponKata)
        {
            if (flyweight.combos[index].weapon == null)
                weaponKata.ChangeWeapon(weapons.actual.equiped);
            else
            {
                var aux2 = (MeleeWeapon)flyweight.combos[index].weapon.Create();
                aux2.Init(inventoryEntity);
                aux2.isDefault = flyweight.kataCombos[index].isDefault;
                //combos.actual.SetDefaultItem(aux);
                weaponKata.ChangeWeapon(aux2);
            }
        }
    }

    void TriggerOnEquipMediator<T>(SlotItemList<T> SlotItemList, int indexSlot , int indexItem, T item) where T : ItemEquipable 
    {
        int indexSlotType = -1;

        if (typeof(SlotItemList<MeleeWeapon>) == typeof(SlotItemList<T>))
        {
            indexSlotType = 0;
        }
        else if(typeof(SlotItemList<WeaponKata>) == typeof(SlotItemList<T>))
        {
            indexSlotType = 1;
        }
        else if(typeof(SlotItemList<Ability>) == typeof(SlotItemList<T>))
        {
            indexSlotType = 2;
        }

        indexSlotType *= 1000;

        indexSlotType += indexSlot;

        if (item is MeleeWeapon weapon)
        {
            onEquipInSlotWeapon?.Invoke(indexSlotType, weapon);
        }
        else if(item is WeaponKata Kata)
        {
            Kata.onEquipedWeapon += (w) =>
            {
                onEquipInSlotWeapon?.Invoke((indexSlotType, indexSlot).GetHashCode(), w);
            };
        }
    }

    public void SetAbility(AttackBase.AbilityToEquip abilityToEquip)
    {
        if (abilityToEquip == null || abilityToEquip.indexToEquip > abilities.Count || abilities.Actual(abilityToEquip.indexToEquip).equiped != null)
            return;

        var aux = abilityToEquip.ability.Create();

        aux.Init(inventoryEntity);

        ((AbilityExtCast)aux).isDefault = abilityToEquip.isDefault;

        abilities.actual.SetDefaultItem((AbilityExtCast)aux);

        ((AbilityExtCast)aux).CreateCopy(out int otherindex);

        abilities.actual.isModifiable = abilityToEquip.isModifiable;
        abilities.actual.isBlocked = abilityToEquip.isBlocked;
        abilities.actual.indexEquipedItem = otherindex;
    }


    #region seteo de la entidad

    public override void OnStayState(Entity param)
    {
        //EnergyUpdate();
        //Debug.Log($"La energia positiva es: {PositiveEnergy} y la negativa es: {NegativeEnergy}");
    }

    public override void OnExitState(Entity param)
    {
        container = null;
        inventoryEntity = null;
        aimingSystem = null;
        enabled = false;
    }

    public override void OnEnterState(Entity param)
    {
        inventoryEntity = param.GetInContainer<InventoryEntityComponent>();
        aimingSystem = param.GetInContainer<AimingEntityComponent>();

        weapons.Init(inventoryEntity, TriggerOnEquipMediator);
        abilities.Init(inventoryEntity, TriggerOnEquipMediator);
        katas.Init(inventoryEntity, TriggerOnEquipMediator);
        combos.Init(inventoryEntity, TriggerOnEquipMediator);
        //abilitiesCombo.Init(inventoryEntity);

        additiveDamage = new DamageContainer(() => flyweight?.additiveDamage);

        if (flyweight == null)
        {
            flyweight = container.flyweight?.GetFlyWeight<AttackBase>();
        }

        positiveEnergy = EnergyDefault * MaxEnergy;

        if (flyweight?.weaponToEquip != null)
            SetWeapon(0);

        if (flyweight?.kataCombos != null)
            for (int i = 0; i < Mathf.Clamp(flyweight.kataCombos.Length, 0, katas.Count); i++)
            {
                SetWeaponKata(i);
            }

        if (flyweight?.abilities != null)
            for (int i = 0; i < Mathf.Clamp(flyweight.abilities.Length, 0, abilities.Count); i++)
            {
                SetAbility(i);
            }

        if (flyweight?.combos != null)
            for (int i = 0; i < Mathf.Clamp(flyweight.combos.Length, 0, combos.Count); i++)
            {
                SetCombo(i);
            }
    }

    #endregion

    public void Init()
    {
        for (int i = 0; i < katas.Count; i++)
        {
            katas[i].inventoryComponent = inventoryEntity;

            if (katas[i].equiped != null)
            {
                katas[i].equiped.Init(inventoryEntity);
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

