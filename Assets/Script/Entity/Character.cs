using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSMCharacterAndStates;
using UnityEngine.EventSystems;
using System;

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

    [field: SerializeField]
    public int actualCombo { get; private set; } = -2;

    [SerializeField]
    int nextCombo = -1;
    [SerializeField]
    float deltaTimeCombo = 0.3f;

    float timeComboSet;

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

    public void Attack(int i, Vector2 dir)
    {
        caster.OnEnterCasting += () => attackEventMediator += caster.abilityControllerMediator;
        caster.Attack(i, dir);
        caster.OnExitCasting += () => attackEventMediator -= caster.abilityControllerMediator;
        Action = castingActionCharacter;
        actualCombo = -1;
    }

    public void Ability(int i, Vector2 dir)
    {
        caster.OnEnterCasting += () => abilityEventMediator += caster.abilityControllerMediator;
        caster.Ability(i, dir);
        caster.OnExitCasting += () => abilityEventMediator -= caster.abilityControllerMediator;
        Action = castingActionCharacter;
    }

    public void AlternateAbility(Vector2 dir)
    {
        caster.OnEnterCasting += () => dashEventMediator += caster.abilityControllerMediator;
        caster.AlternateAbility(dir);
        caster.OnExitCasting += () => dashEventMediator -= caster.abilityControllerMediator;
        Action = castingActionCharacter;
    }

    public void ComboAttack(int i)
    {
        nextCombo = i;
        timeComboSet = Time.realtimeSinceStartup;
    }

    public void ComboAttack()
    {
        actualCombo = -2;

        if (nextCombo < 0)
            return;

        if (Time.realtimeSinceStartup - timeComboSet > deltaTimeCombo)
        {
            nextCombo = -1;
            return;
        }


        actualCombo = nextCombo;


        Debug.Log($"Combo Nº{(actualCombo/5)+1}\nReal index:{actualCombo} \n{Time.realtimeSinceStartup} - {timeComboSet} = {Time.realtimeSinceStartup - timeComboSet > deltaTimeCombo}");
        caster.OnEnterCasting += () => attackEventMediator += caster.abilityControllerMediator;
        caster.ComboAttack(actualCombo);
        caster.OnExitCasting += () => attackEventMediator -= caster.abilityControllerMediator;
        
        Action = castingActionCharacter;

        timeComboSet = 0;
        nextCombo = -1;
    }

    private void Health_death()
    {
        transform.SetActiveGameObject(false);
    }

    protected override void Config()
    {
        MyAwakes += MyAwake;
        MyStarts += MyStart;
        MyOnEnables += MyEnables;
        MyOnDisables += MyDisables;
        base.Config();
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
        actualCombo = -2;

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

        //Debug.Log($"Da�o: {damage} queda: {CurrentDefense}");

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
            //Debug.Log("Se da�o la defensa de: " + gameObject.name);
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

            param.context.ComboAttack();
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
                if (_stateWithEnd != null)
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

        FSMAutomaticEnd<Character> InternalCharacterAction = new FSMAutomaticEnd<Character>(); //propia de action

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
        CasterEntityComponent caster;

        public bool End => caster?.End ?? true;

        public void OnEnterState(FSMAutomaticEnd<Character> param)
        {
            caster = param.context.caster;
        }

        public void OnStayState(FSMAutomaticEnd<Character> param)
        {
            caster.abilityCasting.OnStayState(caster);
        }

        public void OnExitState(FSMAutomaticEnd<Character> param)
        {
            caster.abilityCasting = null;
            caster = null;
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