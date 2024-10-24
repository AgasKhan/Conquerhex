using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    string originalTag;

    [SerializeField]
    Detect<Interactuable> detectInteractuable = new Detect<Interactuable>();

    Interactuable lastInteractuable;

    ControllerIAIO prin;

    ControllerIAIO sec;

    ControllerIAIO ter;

    EventJoystick interactEvent;

    private void Awake()
    {
        interactEvent = EventManager.events.SearchOrCreate<EventJoystick>(EnumController.interact.ToString());

        prin = new ControllerIAIO(EnumController.principal, 0);

        sec = new ControllerIAIO(EnumController.secondary, 1);

        ter = new ControllerIAIO(EnumController.terciary, 2);

        LoadSystem.AddPreLoadCorutine(() => {
            OnExitState(_character);
        });
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

        prin.Init();
        sec.Init();
        ter.Init();

        VirtualControllers.movement.SuscribeController(param.move);

        VirtualControllers.principal.SuscribeController(prin);

        VirtualControllers.secondary.SuscribeController(sec);

        VirtualControllers.terciary.SuscribeController(ter);
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

        VirtualControllers.movement.DesuscribeController(param.move);

        VirtualControllers.principal.DesuscribeController(prin);

        VirtualControllers.secondary.DesuscribeController(sec);

        VirtualControllers.terciary.DesuscribeController(ter);

        prin.Exit();
        sec.Exit();
        ter.Exit();

        VirtualControllers.interact.eventDown -= Interact_eventDown;

        interactEvent.ExecuteSet(false, false, null);

        lastInteractuable = null;

        param.gameObject.tag = originalTag;

        base.OnExitState(_character);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        EventManager.events.SearchOrCreate<EventGeneric>("move").Execute(transform.position);

        var buildings = detectInteractuable.Area(transform.position, (edificio) => { return edificio.interactuable; });

        if(buildings == null || buildings.Count == 0)
        {
            interactEvent.ExecuteSet(false, false, null);

            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = null;
        }
        else if (buildings[0] != lastInteractuable)
        {
            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastInteractuable = buildings[0];

            interactEvent.ExecuteSet(true, false, lastInteractuable.Image);

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
            Interfaz.instance?["Titulo secundario"].ShowMsg("Dirigible a la vista");
        }
    }

    protected override void Health_death()
    {
        GameManager.instance.Defeat();
        OnExitState(_character);
    }

    void UpdateLife(IGetPercentage arg1, float arg3)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).Execute(arg1, arg3);
    }

    void UpdateRegen(IGetPercentage arg1, float arg3)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.regen).Execute(arg1, arg3);
    }
}



public class ControllerIAIO : IControllerDir, Init
{
    EventJoystick _Event;

    WeaponKata previusControllerDir;

    Character character;

    int index;

    EquipedItem<WeaponKata> kata => character.ActualKata(index);

    System.Action attackAnim => character.AttackEvent;

    public void ControllerDown(Vector2 dir, float tim)
    {
        kata.equiped?.ControllerDown(dir,tim);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        kata.equiped?.ControllerPressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        kata.equiped?.ControllerUp(dir, tim);
    }

    public void SetJoystick(int arg1, WeaponKata arg2)
    {
        if (previusControllerDir == kata.equiped && kata.equiped!=null)
            return;

        if (previusControllerDir != null)
        {
            previusControllerDir.onCooldownChange -= Ui;
            previusControllerDir.onAttack -= attackAnim;
        }

        if (kata.equiped != null)
        {
            kata.equiped.onCooldownChange += Ui;
            kata.equiped.onAttack += attackAnim;
        }

        RefreshJoystickUI();

        previusControllerDir = kata.equiped;
    }

    void Ui(IGetPercentage f, float num)
    {
        _Event.Execute(f, num);
    }

    void RefreshJoystickUI()
    {
        if (character!=null && kata.equiped != null)
        {
            _Event.ExecuteSet(true, kata.equiped.itemBase.joystick, kata.equiped.image);
        }
        else
        {
            _Event.ExecuteSet(false, false, null);
        }
    }

    public void Init(params object[] param)
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
                kata.equiped.onAttack -= attackAnim;
            }
        }

        character = null;
        previusControllerDir = null;

        RefreshJoystickUI();
    }

    public ControllerIAIO(EnumController enumController, int index)
    {
        _Event = EventManager.events.SearchOrCreate<EventJoystick>(enumController.ToString());

        this.index = index;
    }
}