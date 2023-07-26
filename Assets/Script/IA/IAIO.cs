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
        LoadSystem.AddPreLoadCorutine(() => {
            OnExitState(character);
            interactEvent = EventManager.events.SearchOrCreate<EventJoystick>(EnumController.interact.ToString());
        });
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        GameManager.instance.playerCharacter = param;

        prin = new ControllerIAIO(EnumController.principal, character.ActualKata(0));

        sec = new ControllerIAIO(EnumController.secondary, character.ActualKata(1));

        ter = new ControllerIAIO(EnumController.terciary, character.ActualKata(2));

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;

        param.health.death += ShowLoserWindow;

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

        param.health.death -= ShowLoserWindow;

        VirtualControllers.movement.DesuscribeController(param.move);

        VirtualControllers.principal.DesuscribeController(prin);

        VirtualControllers.secondary.DesuscribeController(sec);

        VirtualControllers.terciary.DesuscribeController(ter);

        VirtualControllers.interact.eventDown -= Interact_eventDown;

        interactEvent.ExecuteSet(false, false, null);

        EventManager.events.SearchOrCreate<EventJoystick>(EnumController.principal.ToString()).ExecuteSet(false, false, null);

        EventManager.events.SearchOrCreate<EventJoystick>(EnumController.secondary.ToString()).ExecuteSet(false, false, null);

        EventManager.events.SearchOrCreate<EventJoystick>(EnumController.terciary.ToString()).ExecuteSet(false, false, null);       

        lastInteractuable = null;

        param.gameObject.tag = originalTag;
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        EventManager.events.SearchOrCreate<EventGeneric>("move").Execute(transform.position);

        var buildings = detectInteractuable.Area(transform.position, (edificio) => { return edificio.enabled; });

        if(buildings == null || buildings.Count == 0)
        {
            interactEvent.ExecuteSet(false, false, null);

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
        lastInteractuable.Interact(character);
    }

    private void TeleportEvent(Hexagone obj, int lado)
    {
        obj.SetRenders(HexagonsManager.LadoOpuesto(lado));

        

        if (HexagonsManager.idMaxLevel == obj.id)
        {
            Interfaz.instance["Titulo secundario"].ShowMsg("Dirigible a la vista");
        }
    }

    void ShowLoserWindow()
    {
        TimersManager.LerpInTime(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            GameManager.instance.Pause(true);
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("Has muerto", "").AddButton("Reiniciar", () => LoadSystem.instance.Reload()).AddButton("Volver a la base", () => LoadSystem.instance.Load("Base"));
        }).SetUnscaled(true);

        OnExitState(character);
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
        if (kata.equiped != null)
        {
            _Event.ExecuteSet(true, kata.equiped.itemBase.joystick, kata.equiped.image);
        }
        else
        {
            _Event.ExecuteSet(false, false, null);
        }
    }

    public ControllerIAIO(EnumController enumController, EquipedItem<WeaponKata> kata)
    {
        _Event = EventManager.events.SearchOrCreate<EventJoystick>(enumController.ToString());

        this.kata = kata;
        this.attackAnim = kata.character.AttackEvent;

        kata.toChange += SetJoystick;

        SetJoystick(0, null);
    }

    ~ControllerIAIO()
    {
        kata.toChange -= SetJoystick;
        kata.equiped.onCooldownChange -= Ui;
        kata.equiped.onAttack -= attackAnim;
    }
}