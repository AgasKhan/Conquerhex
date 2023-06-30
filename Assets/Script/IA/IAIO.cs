using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    string originalTag;

    [SerializeField]
    Detect<Building> detectBuilding = new Detect<Building>();

    Building lastBuilding;

    ControllerIAIO prin;

    ControllerIAIO sec;

    ControllerIAIO ter;

    private void Start()
    {
        LoadSystem.AddPreLoadCorutine(()=> {
            OnExitState(character);
        });
    }

    public override void OnEnterState(Character param)
    {
        GameManager.instance.playerCharacter = param;

        character = param;

        prin = new ControllerIAIO(EnumController.principal, character.ActualKata(0), AttackAnimation);

        sec = new ControllerIAIO(EnumController.secondary, character.ActualKata(1), AttackAnimation);

        ter = new ControllerIAIO(EnumController.terciary, character.ActualKata(2), AttackAnimation);

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;
        param.move.onMove += MoveAnimation;
        param.move.onIdle += IdleAnimation;

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;

        param.health.noLife += ShowLoserWindow;

        VirtualControllers.movement.SuscribeController(param.move);

        VirtualControllers.principal.SuscribeController(prin);

        VirtualControllers.secondary.SuscribeController(sec);

        VirtualControllers.terciary.SuscribeController(ter);
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;
        param.move.onMove -= MoveAnimation;
        param.move.onIdle -= IdleAnimation;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

        param.health.noLife -= ShowLoserWindow;

        VirtualControllers.movement.DesuscribeController(param.move);

        VirtualControllers.principal.DesuscribeController(prin);

        VirtualControllers.secondary.DesuscribeController(sec);

        VirtualControllers.terciary.DesuscribeController(ter);

        param.gameObject.tag = originalTag;
    }

    public override void OnStayState(Character param)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).Execute(transform.position);

        var buildings = detectBuilding.Area(transform.position, (edificio) => { return true; });

        if(buildings == null || buildings.Count == 0)
        {
            EventManager.events.SearchOrCreate<EventJoystick>(EnumController.interact).ExecuteSet(false, false, null);

            lastBuilding = null;
        }
        else if (buildings[0] != lastBuilding)
        {
            VirtualControllers.interact.eventDown -= Interact_eventDown;

            lastBuilding = buildings[0];

            EventManager.events.SearchOrCreate<EventJoystick>(EnumController.interact).ExecuteSet(true, false, lastBuilding.structureBase.image);

            VirtualControllers.interact.eventDown += Interact_eventDown;
        }
    }

    private void Interact_eventDown(Vector2 arg1, float arg2)
    {
        lastBuilding.myBuildSubMenu.Create();
    }


    private void TeleportEvent(Hexagone obj, int lado)
    {
        obj.SetRenders(HexagonsManager.LadoOpuesto(lado));
    }

    void ShowLoserWindow()
    {
        GameManager.instance.Pause(false);
        Manager<ManagerSubMenus>.pic["Principal"].ShowWindow("PopUp");     
    }

    void UpdateLife(IGetPercentage arg1, float arg2, float arg3)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).Execute(arg1, arg2, arg3);
    }

    void UpdateRegen(IGetPercentage arg1, float arg2, float arg3)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.regen).Execute(arg1, arg2, arg3);
    }
}

public enum EnumPlayer
{
    move
}

public class ControllerIAIO : IControllerDir
{
    EventJoystick _Event;

    WeaponKata previusControllerDir;

    System.Action attackAnim;

    //System.Func<WeaponKata> controllerDir;

    EquipedItem<WeaponKata> kata;

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
        if (previusControllerDir != null)
        {
            previusControllerDir.updateTimer -= Ui;
            previusControllerDir.finishTimer -= UiFinish;
            previusControllerDir.onAttack -= attackAnim;
        }

        if (kata.equiped != null)
        {
            kata.equiped.updateTimer += Ui;
            kata.equiped.finishTimer += UiFinish;
            kata.equiped.onAttack += attackAnim;
        }

        RefreshJoystickUI();

        previusControllerDir = kata.equiped;
    }

    void Ui(float f)
    {
        _Event.Execute(f);
    }

    void UiFinish()
    {
        _Event.ExecuteEnd();
    }

    void RefreshJoystickUI()
    {
        if (kata.equiped != null)
        {
            _Event.ExecuteSet(true, kata.equiped.itemBase.joystick, kata.equiped.image);
        }
        else
        {
            _Event.ExecuteSet(false, false, null);
        }
    }

    public ControllerIAIO(EnumController enumController, EquipedItem<WeaponKata> kata, System.Action attackAnim)
    {
        _Event = EventManager.events.SearchOrCreate<EventJoystick>(enumController);

        this.kata = kata;
        this.attackAnim = attackAnim;

        kata.toChange += SetJoystick;

        SetJoystick(0, null);
    }

    ~ControllerIAIO()
    {
        kata.toChange -= SetJoystick;
        kata.equiped.updateTimer -= Ui;
        kata.equiped.finishTimer -= UiFinish;
        kata.equiped.onAttack -= attackAnim;
    }
}