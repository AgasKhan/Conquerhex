using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    [SerializeField]
    EventManager eventsManager;

    string originalTag;

    [SerializeField]
    Detect<InteractEntityComponent> detectInteractuable = new Detect<InteractEntityComponent>();

    [SerializeField]
    string lastCombo;

    string[] combos = new string[] { "↑↑","→→", "←←" , "↓↓" };

    Timer comboReset;

    InteractEntityComponent lastInteractuable;

    DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> interactEvent;

    SingleEvent<Character> characterEvent;


    private void MoveEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        Vector2 tecla = arg1.AproxDir();

        if (tecla != Vector2.zero)
        {
            comboReset.Reset();

            if (lastCombo.Length >= 2)
                lastCombo = new string(lastCombo[^1], 1);

            if (tecla.x > 0)
            {
                lastCombo += "→";
            }
            else if (tecla.x < 0)
            {
                lastCombo += "←";
            }
            else if (tecla.y > 0)
            {
                lastCombo += "↑";
            }
            else if (tecla.y < 0)
            {
                lastCombo += "↓";
            }
        }
    }

    private void AttackEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        for (int i = 0; i < combos.Length; i++)
        {
            if (combos[i] == lastCombo)
            {
                character.Attack(i + 1);
                return;
            }
        }

        character.Attack(0);
    }

    private void AbilityEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        for (int i = 0; i < combos.Length; i++)
        {
            if (combos[i] == lastCombo)
            {
                character.Ability(i + 1);
                return;
            }
        }

        character.Ability(0);
    }

    private void DashEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        character.AlternateAbility();
    }

    

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        GameManager.instance.playerCharacter = param;

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;
        param.health.regenTimeUpdate += UpdateRegenTime;

        param.onTakeDamage += OnTakeDamage;

        param.health.helthUpdate += Health_helthUpdate;

        param.caster.energyUpdate += EnergyUpdate;
        param.caster.leftEnergyUpdate += LeftEnergyUpdate;
        param.caster.rightEnergyUpdate += RightEnergyUpdate;

        param.attackEventMediator.eventDown += AttackEventMediator_eventDown;

        param.abilityEventMediator.eventDown += AbilityEventMediator_eventDown;

        param.moveEventMediator.eventDown += MoveEventMediator_eventDown;

        param.dashEventMediator.eventDown += DashEventMediator_eventDown;

        VirtualControllers.movement.SuscribeController(param.moveEventMediator);

        VirtualControllers.principal.SuscribeController(param.attackEventMediator);

        VirtualControllers.secondary.SuscribeController(param.abilityEventMediator);

        VirtualControllers.terciary.SuscribeController(param.dashEventMediator);
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;
        param.health.regenTimeUpdate -= UpdateRegenTime;
        param.health.helthUpdate -= Health_helthUpdate;
        param.caster.energyUpdate -= EnergyUpdate;
        param.caster.leftEnergyUpdate -= LeftEnergyUpdate;
        param.caster.rightEnergyUpdate -= RightEnergyUpdate;
        param.onTakeDamage -= OnTakeDamage;

        param.attackEventMediator.eventDown -= AttackEventMediator_eventDown;

        param.abilityEventMediator.eventDown -= AbilityEventMediator_eventDown;

        param.moveEventMediator.eventDown -= MoveEventMediator_eventDown;

        param.dashEventMediator.eventDown -= DashEventMediator_eventDown;

        VirtualControllers.movement.DesuscribeController(param.moveEventMediator);

        VirtualControllers.principal.DesuscribeController(param.attackEventMediator);

        VirtualControllers.secondary.DesuscribeController(param.abilityEventMediator);

        VirtualControllers.terciary.DesuscribeController(param.dashEventMediator);

        VirtualControllers.interact.eventDown -= Interact_eventDown;

        interactEvent.secondDelegato?.Invoke((false, false, null));

        lastInteractuable = null;
        param.gameObject.tag = originalTag;

        base.OnExitState(character);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        eventsManager.events.SearchOrCreate<SingleEvent<Vector3>>("move").delegato.Invoke(transform.position);

        var buildings = detectInteractuable.Area(character.transform.position, (edificio) => { return edificio.interactuable; });

        if(buildings == null || buildings.Count == 0)
        {
            interactEvent.secondDelegato?.Invoke((false, false, null));

            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = null;
        }
        else if (buildings[0] != lastInteractuable)
        {
            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = buildings[0];

            interactEvent.secondDelegato?.Invoke((true, false, lastInteractuable.Image));

            VirtualControllers.interact.eventDown += Interact_eventDown;
        }
    }

    private void Interact_eventDown(Vector2 arg1, float arg2)
    {
        lastInteractuable.Interact(character);
    }

    private void TeleportEvent(Hexagone obj, int lado)
    {
        HexagonsManager.SetRenders(obj, HexagonsManager.LadoOpuesto(lado), obj.AristaMasCercana(character.transform));


        if (HexagonsManager.idMaxLevel == obj.id)
        {
            UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Dirigible a la vista");
        }
    }

    protected override void Health_death()
    {
        var chr = character;

        character.CurrentState = null;

        if (chr == GameManager.instance.playerCharacter)
            GameManager.instance.Defeat("Has muerto");
    }

    private void OnTakeDamage(Damage obj)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato?.Invoke(character.health);
    }

    private void Health_helthUpdate(Health obj)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<Health>>(LifeType.all).delegato?.Invoke(obj);
    }

    private void EnergyUpdate(float obj)
    {
        eventsManager.events.SearchOrCreate<TripleEvent<float, float, float>>("EnergyUpdate").delegato?.Invoke(obj);
    }

    private void LeftEnergyUpdate(float obj)
    {
        eventsManager.events.SearchOrCreate<TripleEvent<float, float, float>>("EnergyUpdate").secondDelegato?.Invoke(obj);
    }

    private void RightEnergyUpdate(float obj)
    {
        eventsManager.events.SearchOrCreate<TripleEvent<float, float, float>>("EnergyUpdate").thirdDelegato?.Invoke(obj);
    }

    void UpdateLife(IGetPercentage arg1, float arg3)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<(IGetPercentage, float)>>(LifeType.life).delegato?.Invoke((arg1, arg3));
    }

    void UpdateRegen(IGetPercentage arg1, float arg3)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<(IGetPercentage, float)>>(LifeType.regen).delegato?.Invoke((arg1, arg3));
    }

    private void UpdateRegenTime(IGetPercentage arg1, float arg2)
    {
        eventsManager.events.SearchOrCreate<SingleEvent<(IGetPercentage, float)>>(LifeType.time).delegato?.Invoke((arg1, arg2));
    }

    private void OnCharacterSelected(Character chara)
    {
        if(character!=null)
            character.CurrentState = character.GetComponent<IAFather>();

        chara.CurrentState = this;
    }

    private void NoCharacterSelected(Vector2 arg1, float arg2)
    {
        characterEvent.delegato.Invoke(GameManager.instance.playerCharacter);
        VirtualControllers.principal.eventDown -= NoCharacterSelected;
    }

    private void Update()
    {
        if(character!=null)
        {
            enabled = false;
            return;
        }

        UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Press attack for return to your Body");
    }

    private void Awake()
    {
        comboReset = TimersManager.Create(0.5f, () => lastCombo = string.Empty);
        characterEvent = eventsManager.events.SearchOrCreate<SingleEvent<Character>>("Character");
        interactEvent = eventsManager.events.SearchOrCreate<DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)>>(EnumController.interact.ToString());
    }

    private void Start()
    {
        LoadSystem.AddPostLoadCorutine(() =>
        {
            characterEvent.delegato += OnCharacterSelected;
            VirtualControllers.principal.eventDown += NoCharacterSelected;
        });
    }
}

public class ControllerIAIO : IControllerDir, Init
{
    DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> _Event;

    WeaponKata previusControllerDir;

    Character character;

    int index;

    SlotItem<WeaponKata> kata => character.caster.katasCombo[index];

    public void ControllerDown(Vector2 dir, float tim)
    {
        character.Attack(index);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {

    }

    public void ControllerUp(Vector2 dir, float tim)
    {

    }

    public void SetJoystick(int arg1, WeaponKata arg2)
    {
        if (previusControllerDir == kata.equiped && kata.equiped!=null)
            return;

        if (previusControllerDir != null)
        {
            previusControllerDir.onCooldownChange -= Ui;
        }

        if (kata.equiped != null)
        {
            kata.equiped.onCooldownChange += Ui;
        }

        RefreshJoystickUI();

        previusControllerDir = kata.equiped;
    }


    void Ui(IGetPercentage f, float num)
    {
        _Event.delegato.Invoke((f, num));
    }

    void RefreshJoystickUI()
    {
        if (character != null && kata.equiped != null)
        {
            _Event.secondDelegato.Invoke((true, kata.equiped.itemBase.joystick, kata.equiped.image));
        }
        else
        {
            _Event.secondDelegato.Invoke((false, false, null));
        }
    }

    public void Init()
    {
        character = GameManager.instance.playerCharacter;

        kata.toChange += SetJoystick;

        SetJoystick(0, null);
    }

    public void Exit()
    {
        if(character != null)
        {
            kata.toChange -= SetJoystick;

            if (kata.equiped != null)
            {
                kata.equiped.onCooldownChange -= Ui;
            }
        }

        character = null;
        previusControllerDir = null;

        RefreshJoystickUI();
    }

    public ControllerIAIO(DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> evento, int index)
    {
        _Event = evento;

        this.index = index;
    }
}