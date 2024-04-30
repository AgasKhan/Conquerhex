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

    /// <summary>
    /// estado de la IA actual
    /// </summary>
    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if (_ia == null && value != null)
            {
                MyUpdates += IAUpdate;
            }
            else if (value == null)
            {
                MyUpdates -= IAUpdate;
            }

            _ia?.OnExitState(this);
            _ia = value;
            _ia?.OnEnterState(this);
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

        MyUpdates += fsmCharacter.UpdateState;
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
        _ia.OnStayState(this);
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

    
}

