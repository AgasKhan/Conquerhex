using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMCharacterAndStates;

[RequireComponent(typeof(CasterEntityComponent))]
[RequireComponent(typeof(AimingEntityComponent))]
[RequireComponent(typeof(MoveEntityComponent))]
public class Character : Entity, ISwitchState<Character, IState<Character>>
{
    /// <summary>
    /// Boton de apuntado
    /// </summary>
    public EventControllerMediator aimingEventMediator = new EventControllerMediator();

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

    [field: SerializeField]
    public AimingEntityComponent aiming { get; private set; }

    public MoveStateCharacter moveStateCharacter { get; private set; }
    public ActionStateCharacter actionStateCharacter { get; private set; }
    public CastingActionCharacter castingActionCharacter { get; private set; }
    public StopActionCharacter stopIA { get; private set; }



    [SerializeReference]
    StunBar stunBar = new StunBar();

    [SerializeField]
    bool iaOn = true;
    

    /// <summary>
    /// estado de la IA actual
    /// </summary>
    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if (!iaOn)
            {
                _ia = value;
                return;
            }

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

    public void IAOnOff(bool value)
    {
        if (iaOn == value)
            return;


        if (value)
        {
            CurrentState?.OnEnterState(this);
            IAUpdateEnable(true);

        }
        else
        {
            CurrentState?.OnExitState(this);
            IAUpdateEnable(false);

        }

        iaOn = value;
    }


    public void StopIA()
    {
        Action = stopIA;
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

    private void Health_death()
    {
        transform.SetActiveGameObject(false);
    }

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts += MyStart;
        MyOnEnables += MyEnables;
        MyOnDisables += MyDisables;
    }

    void IAUpdateEnable(bool value)
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

    void MyDisables()
    {
        attackEventMediator.Enabled = false;

        abilityEventMediator.Enabled = false;

        dashEventMediator.Enabled = false;

        moveEventMediator.Enabled = false;
    }

    void IAUpdate()
    {
        if (iaOn)
            _ia?.OnStayState(this);
    }

    void MyStart()
    {
        if (iaOn && _ia != null)
        {
            _ia.OnEnterState(this);
            MyUpdates += IAUpdate;
        }
    }

    void MyEnables()
    {
        attackEventMediator.Enabled = true;

        abilityEventMediator.Enabled = true;

        dashEventMediator.Enabled = true;

        moveEventMediator.Enabled = true;
    }



    void MyAwake()
    {
        _ia = GetComponent<IState<Character>>();

        move = GetInContainer<MoveEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();
        aiming = GetInContainer<AimingEntityComponent>();
        caster = GetInContainer<CasterEntityComponent>();

        moveStateCharacter = new MoveStateCharacter();
        actionStateCharacter = new ActionStateCharacter(this);

        castingActionCharacter = new CastingActionCharacter();
        stopIA = new StopActionCharacter();

        fsmCharacter = new FSMCharacter(this);

        stunBar.InitStunBar(this);

        MyUpdates += fsmCharacter.UpdateState;       
    }
}

[System.Serializable]
public class StunBar
{
    public StunActionCharacter stunAction { get; private set; }


    public BodyBase body;

    public bool isStunned = false;

    float CurrentDefense
    {
        get => _currentDefense;
        set => _currentDefense = Mathf.Clamp(value, 0, maxDefense);
    }

    float _currentDefense;
    float maxDefense => body?.maxDefense ?? 100;

    Timer regenDelayTim;
    Timer regenTim;

    Character character;

    public void RestartDefense()
    {
        CurrentDefense = maxDefense;
    }

    public void Stun(bool value)
    {
        character.IAOnOff(!value);
        isStunned = value;
    }

    void ReceiveStunDamage(float damage)
    {
        if (isStunned)
            return;

        CurrentDefense -= damage;

        //Debug.Log($"Daño: {damage} queda: {CurrentDefense}");

        regenTim.Stop();

        regenDelayTim.Reset();

        if (CurrentDefense <= 0)
        {
            character.Action = stunAction;
        }
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

    public void InitStunBar(Character character)
    {
        this.character = character;

        this.body = character.flyweight.GetFlyWeight<BodyBase>();

        stunAction = new StunActionCharacter(this);

        character.onTakeDamage += Character_onTakeDamage;

        character.health.reLife += Health_reLife;

        CurrentDefense = maxDefense;

        regenDelayTim = TimersManager.Create(body.defenseRegenDelay, () => regenTim.Reset()).SetInitCurrent(body.defenseRegenDelay).Stop();
        regenTim = TimersManager.Create(body.defenseRegenSpeed, () =>
        {
            CurrentDefense += body.defenseRegenAmount;

            if (CurrentDefense >= maxDefense)
            {
                regenTim.Stop();
            }
        }).SetLoop(true).Stop();
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
        public IStateWithEnd<FSMAutomaticEnd<Character>> stateWithEnd
        {
            get => _stateWithEnd;
            set
            {
                if (_stateWithEnd!=null)
                {
                    //Debug.Log($"ExitAction: {_stateWithEnd.GetType().Name}-------------------------------------------------------------------");

                    InternalCharacterAction.CurrentState = value;

                    _stateWithEnd = value;

                    //Debug.Log($"EnterAction: {_stateWithEnd.GetType().Name}-------------------------------------------------------------------");
                }
                else
                    _stateWithEnd = value;
            }
        }

        IStateWithEnd<FSMAutomaticEnd<Character>> _stateWithEnd;

        FSMAutomaticEnd<Character> InternalCharacterAction = new FSMAutomaticEnd<Character>();

        public void OnEnterState(FSMCharacter param)
        {
            InternalCharacterAction.EnterState(stateWithEnd);
            //Debug.Log($"{param.context.name} EnterAction: {stateWithEnd.GetType().Name}-------------------------------------------------------------------");
        }

        public void OnStayState(FSMCharacter param)
        {
            InternalCharacterAction.UpdateState();
            if (InternalCharacterAction.end)
                param.CurrentState = param.context.moveStateCharacter;
        }

        public void OnExitState(FSMCharacter param)
        {
            stateWithEnd.OnExitState(InternalCharacterAction);

            //Debug.Log($"{param.context.name} ExitAction: {stateWithEnd.GetType().Name}-------------------------------------------------------------------");

            _stateWithEnd = null;
        }

        public ActionStateCharacter(Character character)
        {
            InternalCharacterAction.Init(character);
        }
    }

    public class StopActionCharacter : IStateWithEnd<FSMAutomaticEnd<Character>>
    {
        public bool End => false;
        public event System.Action OnStopIA;

        public void OnEnterState(FSMAutomaticEnd<Character> param)
        {
            UI.Interfaz.instance.PopText(param.context, "Apagado".RichText("size", "35").RichTextColor(Color.red), Vector2.up * 2);
            param.context.IAOnOff(false);

            OnStopIA?.Invoke();
        }

        public void OnStayState(FSMAutomaticEnd<Character> param)
        {

        }

        public void OnExitState(FSMAutomaticEnd<Character> param)
        {
            param.context.IAOnOff(true);
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
            _stateWithEnd?.OnExitState(param.context.caster);

            this.param = null;

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

        StunBar stunBar;

        public void OnEnterState(FSMAutomaticEnd<Character> param)
        {
            UI.Interfaz.instance.PopText(param.context, "Stunned".RichText("size", "45").RichTextColor(Color.white), Vector2.up * 2);
            stunBar.Stun(true);

            stunTimer.Reset();
            OnStunned?.Invoke();

            //Debug.Log(param.context.name + " Stunned-------------------------------------------------------------------");
        }

        public void OnStayState(FSMAutomaticEnd<Character> param)
        {

        }

        public void OnExitState(FSMAutomaticEnd<Character> param)
        {
            stunBar.isStunned = false;
            stunBar.Stun(false);

            stunBar.RestartDefense();
            OnRecover?.Invoke();

            //Debug.Log(param.context.name + " Recovered-------------------------------------------------------------------");
        }

        public StunActionCharacter(StunBar stunBar)
        {
            this.stunBar = stunBar;
            stunTimer = TimersManager.Create(stunBar.body.stunTime).Stop();
        }
    }
}