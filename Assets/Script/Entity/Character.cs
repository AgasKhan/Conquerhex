using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMCharacterAndStates;

[RequireComponent(typeof(MoveEntityComponent))]
[RequireComponent(typeof(CasterEntityComponent))]
public class Character : Entity, ISwitchState<Character, IState<Character>>
{
    /// <summary>
    /// Boton de ataque
    /// </summary>
    public EventControllerMediator attackEventMediator = new EventControllerMediator();

    /// <summary>
    /// Boton de habilidad
    /// </summary>
    public EventControllerMediator abilityEventMediator = new EventControllerMediator();

    /// <summary>
    /// Boton de dash/tercera habilidad
    /// </summary>
    public EventControllerMediator dashEventMediator = new EventControllerMediator();

    /// <summary>
    /// Boton movimiento
    /// </summary>
    public EventControllerMediator moveEventMediator = new EventControllerMediator();
    
    IState<Character> _ia;

    FSMCharacter fsmCharacter;

    [field: SerializeField]
    public InventoryEntityComponent inventory { get; private set; }

    [field: SerializeField]
    public CasterEntityComponent caster { get; private set; }

    [field: SerializeField]
    public MoveEntityComponent move { get; private set; }

    public ActionStateCharacter actionStateCharacter { get; private set; }
    public CastingActionCharacter castingActionCharacter { get; private set; }
    public MoveStateCharacter moveStateCharacter { get; private set; }
    public StunActionCharacter stunAction { get; private set; }

    public System.Action<(Timer, ItemBase)>[] equipedEvents = new System.Action<(Timer, ItemBase)>[12];

    public bool isStunned = false;

    public float CurrentDefense
    {
        get => _currentDefense;
        set => _currentDefense = Mathf.Clamp(value, 0, maxDefense);
    }

    float _currentDefense;
    float maxDefense = 100f;

    Timer regenDelayTim;
    Timer regenTim;

    /// <summary>
    /// estado de la IA actual
    /// </summary>
    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if (value == null)
            {
                IAUpdateEnable(false);
            }
            else if (_ia == null && value != null)
            {
                IAUpdateEnable(true);
            }

            _ia?.OnExitState(this);
            _ia = value;
            _ia?.OnEnterState(this);
        }
    }

    public void IAUpdateEnable(bool value)
    {
        if (value)
        {
            MyUpdates += IAUpdate;
        }
        else
        {
            MyUpdates -= IAUpdate;
        }
    }

    public IStateWithEnd<FSMAutomaticEnd<Character>> Action
    {
        set
        {
            actionStateCharacter.stateWithEnd = value;

            fsmCharacter.CurrentState = actionStateCharacter;
        }
    }

    public void Attack(int i)
    {
        WeaponKata weaponKata;

        if (i == 0)
        {
            weaponKata = caster.actualWeapon;
        }
        else
        {
            weaponKata = caster.katasCombo.Actual(i - 1).equiped;
        }

        if (castingActionCharacter.stateWithEnd == weaponKata)
            return;

        castingActionCharacter.OnEnter+= () => attackEventMediator += caster.abilityControllerMediator;
        castingActionCharacter.OnExit += () => attackEventMediator -= caster.abilityControllerMediator;

        castingActionCharacter.stateWithEnd = weaponKata;

        Action = castingActionCharacter;
    }

    public void Ability(int i)
    {
        if (i != 0)
        {
            i += 1;
        }

        AbilityExtCast weaponKata;

        weaponKata = caster.abilities.Actual(i).equiped;

        if (castingActionCharacter.stateWithEnd == weaponKata)
            return;

        castingActionCharacter.OnEnter += () => abilityEventMediator += caster.abilityControllerMediator;
        castingActionCharacter.OnExit += () => abilityEventMediator -= caster.abilityControllerMediator;

        castingActionCharacter.stateWithEnd = weaponKata;

        Action = castingActionCharacter;

    }

    public void AlternateAbility()
    {
        AbilityExtCast weaponKata;

        weaponKata = caster.abilities.Actual(1).equiped;

        if (castingActionCharacter.stateWithEnd == weaponKata)
            return;

        castingActionCharacter.OnEnter += () => dashEventMediator += caster.abilityControllerMediator;
        castingActionCharacter.OnExit += () => dashEventMediator -= caster.abilityControllerMediator;

        castingActionCharacter.stateWithEnd = weaponKata;

        Action = castingActionCharacter;
    }

    public void TriggerUI()
    {
        equipedEvents[0].Invoke((caster.weapons[0].equiped?.defaultKata.cooldown, caster.weapons[0].equiped?.itemBase));
        equipedEvents[1].Invoke((caster.abilities[0].equiped?.cooldown, caster.abilities[0].equiped?.itemBase));
        equipedEvents[2].Invoke((caster.abilities[1].equiped?.cooldown, caster.abilities[1].equiped?.itemBase));

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 3].Invoke((caster.katasCombo[i].equiped?.cooldown, caster.katasCombo[i].equiped?.itemBase));
        }

        for (int i = 0; i < 4 && (i + 2) < caster.abilities.Count; i++)
        {
            equipedEvents[i + 7].Invoke((caster.abilities[i + 2].equiped?.cooldown, caster.abilities[i + 2].equiped?.itemBase));
        }
    }

    public void RestartDefense()
    {
        CurrentDefense = maxDefense;
    }

    void SaveUI()
    {
        caster.weapons[0].toChange += (index, item) => equipedEvents[0]?.Invoke((item?.defaultKata?.cooldown, item?.itemBase));
        caster.abilities[0].toChange += (index, item) => equipedEvents[1]?.Invoke((item?.cooldown, item?.itemBase));
        caster.abilities[1].toChange += (index, item) => equipedEvents[2]?.Invoke((item?.cooldown, item?.itemBase));

        for (int i = 0; i < 4 && (i) < caster.abilities.Count; i++)
        {
            var indexI = i;
            caster.katasCombo[i].toChange += (index, item) => equipedEvents[indexI + 3]?.Invoke((item?.cooldown, item?.itemBase));
        }

        for (int i = 0; i < 4 && (i +2) < caster.abilities.Count; i++)
        {
            var indexI = i;
            caster.abilities[i + 2].toChange += (index, item) => equipedEvents[indexI + 7]?.Invoke((item?.cooldown, item?.itemBase));
        }
        
    }

    void ReceiveStunDamage(float damage)
    {
        if (isStunned)
            return;

        regenTim.Stop();
        CurrentDefense -= damage;
        //Debug.Log("Daño: " + damage);

        if (CurrentDefense <= 0)
        {
            Action = stunAction;
        }
        else
        {
            regenDelayTim.Reset();
        }
    }

    void InitStunBar(float _maxDefense, float _regenDelay, float _regenSpeed, float _regenAmount)
    {
        maxDefense = _maxDefense;
        CurrentDefense = maxDefense;

        regenDelayTim = TimersManager.Create(_regenDelay, () => regenTim.Reset()).Stop();
        regenTim = TimersManager.Create(_regenSpeed, () =>
        {
            if (CurrentDefense < maxDefense)
            {
                CurrentDefense += _regenAmount;
                regenTim.Reset();
            }
        }).Stop();
    }

    private void Health_reLife()
    {
        RestartDefense();
    }
    private void Character_onTakeDamage(Damage dmg)
    {
        if (dmg.typeInstance.target == DamageTypes.Target.defense)
        {
            //Debug.Log("Se daño la defensa de: " + gameObject.name);
            ReceiveStunDamage(dmg.amount);
        }
    }

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts += MyStart;
        MyOnEnables += MyEnables;
        MyOnDisables += MyDisables;
    }

    void MyEnables()
    {
        attackEventMediator.Enabled = true;

        abilityEventMediator.Enabled = true;

        dashEventMediator.Enabled = true;

        moveEventMediator.Enabled = true;
    }

    void MyDisables()
    {
        attackEventMediator.Enabled =false;

        abilityEventMediator.Enabled = false;

        dashEventMediator.Enabled = false;

        moveEventMediator.Enabled = false;
    }

    void MyAwake()
    {
        _ia = GetComponent<IState<Character>>();

        move = GetInContainer<MoveEntityComponent>();
        caster = GetInContainer<CasterEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();

        castingActionCharacter = new CastingActionCharacter();

        actionStateCharacter = new ActionStateCharacter(this);
        moveStateCharacter = new MoveStateCharacter();

        fsmCharacter = new FSMCharacter(this);

        onTakeDamage += Character_onTakeDamage;

        var body = flyweight.GetFlyWeight<BodyBase>();
        stunAction = new StunActionCharacter(body.stunTime);
        InitStunBar(body.maxDefense, body.defenseRegenDelay, body.defenseRegenSpeed, body.defenseRegenAmount);

        health.reLife += Health_reLife;

        MyUpdates += fsmCharacter.UpdateState;

        //Transladar luego a IAIO toda la logica de UI
        SaveUI();
    }

    void MyStart()
    {
        if (_ia != null)
        {
            _ia.OnEnterState(this);
            MyUpdates += IAUpdate;
        }
    }

    void IAUpdate()
    {
        if(!isStunned)
            _ia?.OnStayState(this);
    }
}

/// <summary>
/// Fsm y estados del character
/// </summary>
namespace FSMCharacterAndStates
{
    /// <summary>
    /// Fsm dedicada a managear el estado del character
    /// </summary>
    public class FSMCharacter : FSM<FSMCharacter, Character>
    {
        public FSMCharacter(Character reference) : base(reference)
        {
            Init(reference.moveStateCharacter);
        }
    }

    /// <summary>
    /// Estado por defecto del personaje
    /// </summary>
    public class MoveStateCharacter : IState<FSMCharacter>
    {
        public event System.Action OnActionEnter;

        public event System.Action OnActionExit;

        public void OnEnterState(FSMCharacter param)
        {
            param.context.moveEventMediator += param.context.move;

            OnActionEnter?.Invoke();
        }

        public void OnExitState(FSMCharacter param)
        {
            param.context.moveEventMediator -= param.context.move;

            OnActionExit?.Invoke();
        }
        public void OnStayState(FSMCharacter param)
        {
        }
    }

    /// <summary>
    /// Estado del character dedicado a realizar una accion generica que cuando termina vuelve al estado de move
    /// </summary>
    public class ActionStateCharacter : IState<FSMCharacter>
    {
        public IStateWithEnd<FSMAutomaticEnd<Character>> stateWithEnd;

        FSMAutomaticEnd<Character> InternalCharacterAction = new FSMAutomaticEnd<Character>();

        public void OnEnterState(FSMCharacter param)
        {
            InternalCharacterAction.EnterState(stateWithEnd);
        }

        public void OnStayState(FSMCharacter param)
        {
            InternalCharacterAction.UpdateState();
            if (InternalCharacterAction.end)
                param.CurrentState = param.context.moveStateCharacter;
        }

        public void OnExitState(FSMCharacter param)
        {
            if(!InternalCharacterAction.end)
                stateWithEnd.OnExitState(InternalCharacterAction);
        }

        public ActionStateCharacter(Character character)
        {
            InternalCharacterAction.Init(character);
        }
    }

    /// <summary>
    /// Estado del character dedicado a realizar el casteo de una habilidad
    /// </summary>
    public class CastingActionCharacter : IStateWithEnd<FSMAutomaticEnd<Character>>
    {
        IStateWithEnd<CasterEntityComponent> _stateWithEnd;

        FSMAutomaticEnd<Character> param;

        System.Action _OnEnter;

        System.Action _OnExit;

        public bool End => stateWithEnd?.End ?? true;

        //public bool End => stateWithEnd == null ? true : false;

        public event System.Action OnEnter
        {
            add
            {
                //_OnEnter = _OnEnter.AddUniqueExecution(value);

                System.Action action = null;

                action = () =>
                {
                    value();
                    //_OnEnter -= value;
                    _OnEnter -= action;
                };

                _OnEnter += action;
            }
            remove
            {
            }
        }

        public event System.Action OnExit
        {
            add
            {
                System.Action action = null;

                action = () =>
                {
                    value();
                    //_OnExit -= value;
                    _OnExit -= action;
                };

                _OnExit += action;
            }
            remove
            {
            }
        }

        public IStateWithEnd<CasterEntityComponent> stateWithEnd
        {
            get => _stateWithEnd;

            set
            {
                if (param != null)
                {
                    stateWithEnd.OnExitState(param.context.caster);

                    _OnExit?.Invoke();

                    _stateWithEnd = value;

                    _OnEnter?.Invoke();

                    stateWithEnd?.OnEnterState(param.context.caster);
                }
                else
                    _stateWithEnd = value;
            }
        }

        public void OnEnterState(FSMAutomaticEnd<Character> param)
        {
            this.param = param;

            _OnEnter?.Invoke();

            stateWithEnd?.OnEnterState(param.context.caster);
        }

        public void OnStayState(FSMAutomaticEnd<Character> param)
        {
            stateWithEnd?.OnStayState(param.context.caster);
        }

        public void OnExitState(FSMAutomaticEnd<Character> param)
        {
            this.param = null;
            _stateWithEnd?.OnExitState(param.context.caster);

            _stateWithEnd = null;

            _OnExit?.Invoke();
        }
    }

    public class StunActionCharacter : IStateWithEnd<FSMAutomaticEnd<Character>>
    {
        public bool End => stunTimer.Chck;

        Timer stunTimer;

        public event System.Action OnStunned;
        public event System.Action OnRecover;

        Character character;

        public void OnEnterState(FSMAutomaticEnd<Character> param)
        {
            character = param.context;

            UI.Interfaz.instance.PopText(param.context, "Stunned".RichText("size", "45").RichTextColor(Color.black), Vector2.up * 2);

            param.context.IAUpdateEnable(false);
            param.context.CurrentState.OnExitState(param.context);
            
            character.isStunned = true;
            stunTimer.Reset();
            OnStunned?.Invoke();

            stunTimer.Reset();

            //Debug.Log(character.gameObject.name + " Stunned-------------------------------------------------------------------");
        }

        public void OnStayState(FSMAutomaticEnd<Character> param)
        {

        }

        public void OnExitState(FSMAutomaticEnd<Character> param)
        {
            character.isStunned = false;
            character.RestartDefense();
            OnRecover?.Invoke();

            param.context.CurrentState.OnEnterState(param.context);
            param.context.IAUpdateEnable(true);
            //Debug.Log(character.gameObject.name + " Recovered-------------------------------------------------------------------");
        }

        public StunActionCharacter(float _stunTime)
        {
            stunTimer = TimersManager.Create(_stunTime).SetInitCurrent(_stunTime).Stop();
        }
    }
}
/*
[System.Serializable]
public class StunBar
{
    public float maxDefense = 100f;
    public bool isStunned = false;

    public event System.Action OnStunned;
    public event System.Action OnRecover;

    public float CurrentDefense 
    { 
        get => _currentDefense; 
        set => _currentDefense = Mathf.Clamp(value, 0, maxDefense); 
    }
    float _currentDefense;
    
    Timer regenDelayTim;
    Timer regenTim;
    Timer stunTimer;

    public void ReceiveStunDamage(float damage)
    {
        if (isStunned)
            return;

        regenTim.Stop();
        CurrentDefense -= damage;
        Debug.Log("Daño: " + damage);

        if (CurrentDefense <= 0)
        {
            StunCharacter();
        }
        else
        {
            regenDelayTim.Reset();
        }
    }

    void StunCharacter()
    {
        isStunned = true;
        stunTimer.Reset();
        OnStunned?.Invoke();
    }

    void RecoverEnemy()
    {
        isStunned = false;
        CurrentDefense = maxDefense;
        OnRecover?.Invoke();
    }

    public void RestartDefense()
    {
        CurrentDefense = maxDefense;
    }

    public void Init(float _maxDefense, float _stunTime, float _regenDelay, float _regenSpeed, float _regenAmount)
    {
        maxDefense = _maxDefense;
        CurrentDefense = maxDefense;

        regenDelayTim = TimersManager.Create(_regenDelay, () => regenTim.Reset()).Stop();
        regenTim = TimersManager.Create(_regenSpeed, () => 
        {
            if(CurrentDefense < maxDefense)
            {
                CurrentDefense += _regenAmount;
                regenTim.Reset();
            }
        }).Stop();

        stunTimer = TimersManager.Create(_stunTime, () => RecoverEnemy()).Stop();
    }
}*/