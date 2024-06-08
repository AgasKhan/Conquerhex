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

    public System.Action<(Timer, ItemBase)>[] equipedEvents = new System.Action<(Timer, ItemBase)>[12];

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
                MyUpdates -= IAUpdate; 
            }
            else if (_ia == null && value != null)
            {
                MyUpdates += IAUpdate;
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

    
}

[System.Serializable]
public class StunBar
{
    public float maxStun = 100f;
    private float currentStun;
    private bool isStunned = false;

    public float regenDelay = 3f;
    public float regenSpeed = 5f;

    public event System.Action OnStunned;

    Timer regenDelayTim;
    Timer regenTim;

    private void Start()
    {
        currentStun = maxStun;
        regenDelayTim = TimersManager.Create(regenDelay, () => regenTim.Reset());
        regenTim = TimersManager.Create(100, () => currentStun += Time.deltaTime * regenSpeed, null);
    }

    private void Update()
    {
        if (!isStunned && currentStun < maxStun)
        {
            currentStun += regenSpeed * Time.deltaTime;
            currentStun = Mathf.Clamp(currentStun, 0, maxStun);
        }
    }

    public void ReceiveStunDamage(float damage)
    {
        if (isStunned) return;

        currentStun -= damage;
        currentStun = Mathf.Clamp(currentStun, 0, maxStun);

        if (currentStun <= 0)
        {
            StunCharacter();
        }
        else
        {
            regenDelayTim.Reset();
        }
    }

    private void StunCharacter()
    {
        isStunned = true;
        OnStunned?.Invoke();
    }

    private IEnumerator StartRegenerationAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);
        isStunned = false;
    }
}