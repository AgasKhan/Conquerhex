using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    string originalTag;

    [SerializeField]
    Detect<Building> detectBuilding = new Detect<Building>();

    Building lastBuilding;

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

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;
        param.move.onMove += MoveAnimation;
        param.move.onIdle += IdleAnimation;

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;

        param.health.noLife += ShowLoserWindow;            

        SetJoystick(param.prin, VirtualControllers.principal, EnumController.principal, PrinUi, PrinUiFinish);

        SetJoystick(param.sec, VirtualControllers.secondary, EnumController.secondary, SecUi, SecUiFinish);

        SetJoystick(param.ter, VirtualControllers.terciary, EnumController.terciary, TerUi, TerUiFinish);



        VirtualControllers.movement.SuscribeController(param.move);
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;
        param.move.onMove -= MoveAnimation;
        param.move.onIdle -= IdleAnimation;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

        param.health.noLife -= ShowLoserWindow;

        //if (param.prin.itemBase != null)
        VirtualControllers.principal.DesuscribeController(param.prin);
        param.prin.updateTimer -= PrinUi;
        param.prin.finishTimer -= PrinUiFinish;
        param.prin.onAttack -= AttackAnimation;

        //if (param.sec.itemBase != null)
        VirtualControllers.secondary.DesuscribeController(param.sec);
        param.sec.updateTimer -= SecUi;
        param.sec.finishTimer -= SecUiFinish;
        param.sec.onAttack -= AttackAnimation;

        //if (param.ter.itemBase != null)
        VirtualControllers.terciary.DesuscribeController(param.ter);
        param.ter.updateTimer -= TerUi;
        param.ter.finishTimer -= TerUiFinish;
        param.ter.onAttack -= AttackAnimation;

        VirtualControllers.movement.DesuscribeController(param.move);

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

    private void SetJoystick(WeaponKata weaponKata, VirtualControllers.AxisButton axisButton, EnumController enumController, System.Action<float> ui, System.Action uifinish)
    {
        if (weaponKata != null)
        {
            axisButton.SuscribeController(weaponKata);
            weaponKata.updateTimer += ui;
            weaponKata.finishTimer += uifinish;
            weaponKata.onAttack += AttackAnimation; 
            EventManager.events.SearchOrCreate<EventJoystick>(enumController).ExecuteSet(true, weaponKata.itemBase.joystick, weaponKata.itemBase.image);
        }
        else
        {
            EventManager.events.SearchOrCreate<EventJoystick>(enumController).ExecuteSet(false, false, null);
        }
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

    void PrinUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumController.principal).Execute(f);
    }

    void SecUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumController.secondary).Execute(f);
    }

    void TerUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumController.terciary).Execute(f);
    }

    void PrinUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(EnumController.principal).ExecuteEnd();
    }

    void SecUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(EnumController.secondary).ExecuteEnd();
    }

    void TerUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(EnumController.terciary).ExecuteEnd();
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