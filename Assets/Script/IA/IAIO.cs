using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    [SerializeField]
    NewEventManager eventsManager;

    string originalTag;

    [SerializeField]
    Detect<Interactuable> detectInteractuable = new Detect<Interactuable>();

    [SerializeField]
    string lastCombo;

    string[] combos = new string[] { "↑↑","→→", "←←" , "↓↓" };

    Timer comboReset;

    Interactuable lastInteractuable;

    //ControllerIAIO prin;

    //ControllerIAIO sec;

    //ControllerIAIO ter;

    EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)> interactEvent;

    private void Awake()
    {

        //prin = new ControllerIAIO(eventsManager.events.SearchOrCreate<EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)>>(EnumController.principal.ToString()), 0);

        //sec = new ControllerIAIO(eventsManager.events.SearchOrCreate<EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)>>(EnumController.secondary.ToString()), 1);

        //ter = new ControllerIAIO(eventsManager.events.SearchOrCreate<EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)>>(EnumController.terciary.ToString()), 2);

        interactEvent = eventsManager.events.SearchOrCreate<EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)>>(EnumController.interact.ToString());
        LoadSystem.AddPreLoadCorutine(() => {
            OnExitState(_character);
        });

        comboReset = TimersManager.Create(0.5f, () => lastCombo = string.Empty);
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        GameManager.instance.playerCharacter = param;

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.move.onTeleport += TeleportEvent;

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;
        param.health.regenTimeUpdate += UpdateRegenTime;

        param.health.helthUpdate += Health_helthUpdate;

        //prin.Init();
        //sec.Init();
        //ter.Init();

        param.attackEventMediator.eventDown += AttackEventMediator_eventDown;

        param.moveEventMediator.eventDown += MoveEventMediator_eventDown;

        VirtualControllers.movement.SuscribeController(param.moveEventMediator);

        VirtualControllers.principal.SuscribeController(param.attackEventMediator);

        VirtualControllers.secondary.SuscribeController(param.abilityEventMediator);

        //VirtualControllers.terciary.SuscribeController(ter);
    }

    private void MoveEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        Vector2 tecla = arg1.AproxDir();

        if(tecla!=Vector2.zero)
        {
            comboReset.Reset();

            if (lastCombo.Length >= 2)
                lastCombo = new string(lastCombo[^1],1);

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
            if(combos[i] == lastCombo)
            {
                character.Attack(i+1);
                return;
            }
        }

        character.Attack(0);
    }

    public override void OnExitState(Character param)
    {
        param.move.move.onTeleport -= TeleportEvent;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;
        param.health.regenTimeUpdate -= UpdateRegenTime;
        param.health.helthUpdate -= Health_helthUpdate;


        VirtualControllers.movement.DesuscribeController(param.moveEventMediator);

        VirtualControllers.principal.DesuscribeController(param.attackEventMediator);

        VirtualControllers.secondary.DesuscribeController(param.abilityEventMediator);

        //VirtualControllers.terciary.DesuscribeController(ter);

        //prin.Exit();
        //sec.Exit();
        //ter.Exit();

        VirtualControllers.interact.eventDown -= Interact_eventDown;

        interactEvent.secondDelegato.Invoke((false, false, null));

        lastInteractuable = null;
        param.gameObject.tag = originalTag;

        base.OnExitState(_character);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        eventsManager.events.SearchOrCreate<EventParam<Vector3>>("move").delegato.Invoke(transform.position);

        var buildings = detectInteractuable.Area(transform.position, (edificio) => { return edificio.interactuable; });

        if(buildings == null || buildings.Count == 0)
        {
            interactEvent.secondDelegato.Invoke((false, false, null));

            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = null;
        }
        else if (buildings[0] != lastInteractuable)
        {
            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = buildings[0];

            interactEvent.secondDelegato.Invoke((true, false, lastInteractuable.Image));

            VirtualControllers.interact.eventDown += Interact_eventDown;
        }
    }

    private void Interact_eventDown(Vector2 arg1, float arg2)
    {
        lastInteractuable.Interact(_character);
    }

    private void TeleportEvent(Hexagone obj, int lado)
    {
        obj.SetRenders(HexagonsManager.LadoOpuesto(lado));

        

        if (HexagonsManager.idMaxLevel == obj.id)
        {
            UI.Interfaz.instance?["Titulo secundario"].ShowMsg("Dirigible a la vista");
        }
    }

    protected override void Health_death()
    {
        GameManager.instance.Defeat();
        OnExitState(_character);
    }

    private void Health_helthUpdate(Health obj)
    {
        eventsManager.events.SearchOrCreate<EventParam<Health>>(LifeType.all).delegato?.Invoke(obj);
    }


    void UpdateLife(IGetPercentage arg1, float arg3)
    {
        eventsManager.events.SearchOrCreate<EventParam<IGetPercentage, float>>(LifeType.life).delegato?.Invoke(arg1, arg3);
    }

    void UpdateRegen(IGetPercentage arg1, float arg3)
    {
        eventsManager.events.SearchOrCreate<EventParam<IGetPercentage, float>>(LifeType.regen).delegato?.Invoke(arg1, arg3);
    }

    private void UpdateRegenTime(IGetPercentage arg1, float arg2)
    {
        eventsManager.events.SearchOrCreate<EventParam<IGetPercentage, float>>(LifeType.time).delegato?.Invoke(arg1, arg2);
    }
}

public class ControllerIAIO : IControllerDir, Init
{
    EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)> _Event;

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

    public ControllerIAIO(EventTwoParam<(IGetPercentage, float), (bool, bool, Sprite)> evento, int index)
    {
        _Event = evento;

        this.index = index;
    }
}