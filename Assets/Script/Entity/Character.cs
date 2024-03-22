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
    /// Boton movimiento
    /// </summary>
    public EventControllerMediator moveEventMediator = new EventControllerMediator();

    /// <summary>
    /// Boton de dash/tercera habilidad
    /// </summary>
    public EventControllerMediator dashEventMediator = new EventControllerMediator();

    IState<Character> _ia;

    FSMCharacter fsmCharacter;

    [field: SerializeField]
    public InventoryEntityComponent inventory { get; private set; }

    [field: SerializeField]
    public CasterEntityComponent caster { get; private set; }

    [field: SerializeField]
    public MoveEntityComponent move { get; private set; }

    public ActionStateCharacter actionStateCharacter { get; private set; }
    public CastingStateCharacter castingStateCharacter { get; private set; }
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
        caster.PreAttack(i);
        fsmCharacter.CurrentState = castingStateCharacter;
    }

    public void Ability(int i)
    {
        fsmCharacter.CurrentState = castingStateCharacter;
        caster.PreAbility(i);
    }    

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts += MyStart;
        //MyUpdates += fsmCharacter.UpdateState;
    }

    void MyAwake()
    {
        _ia = GetComponent<IState<Character>>();

        move = GetInContainer<MoveEntityComponent>();
        caster = GetInContainer<CasterEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();

        actionStateCharacter = new ActionStateCharacter(this);
        castingStateCharacter = new CastingStateCharacter(this);
        moveStateCharacter = new MoveStateCharacter();

        fsmCharacter = new FSMCharacter(this);
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
        }

        public ActionStateCharacter(Character character)
        {
            InternalCharacterAction.Init(character);
        }
    }

    /// <summary>
    /// Estado del character dedicado a realizar el casteo de una habilidad
    /// </summary>
    public class CastingStateCharacter : IState<FSMCharacter>
    {
        FSMAutomaticEnd<CasterEntityComponent> internalFsm = new FSMAutomaticEnd<CasterEntityComponent>();

        public event System.Action OnActionEnter;

        public event System.Action OnActionExit;

        public CastingStateCharacter(Character character)
        {
            internalFsm.Init(character.caster);
        }

        public void OnEnterState(FSMCharacter param)
        {
            param.context.attackEventMediator += param.context.caster.attack;

            param.context.abilityEventMediator += param.context.caster.ability;

            internalFsm.EnterState(param.context.caster.preState);

            OnActionEnter?.Invoke();
        }

        public void OnStayState(FSMCharacter param)
        {
            internalFsm.UpdateState();
            if (internalFsm.end)
                param.CurrentState = param.context.moveStateCharacter;
        }

        public void OnExitState(FSMCharacter param)
        {
            param.context.attackEventMediator -= param.context.caster.attack;

            param.context.abilityEventMediator -= param.context.caster.ability;

            OnActionExit?.Invoke();
        }
    }

    /// <summary>
    /// Estado por defecto del personaje
    /// </summary>
    public class MoveStateCharacter : IState<FSMCharacter>
    {
        public void OnEnterState(FSMCharacter param)
        {
            param.context.moveEventMediator += param.context.move.move;
        }

        public void OnExitState(FSMCharacter param)
        {
            param.context.moveEventMediator -= param.context.move.move;
        }
        public void OnStayState(FSMCharacter param)
        {

        }
    }
}

