using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{

    string originalTag;

    public override void OnEnterState(Character param)
    {

        GameManager.instance.playerCharacter = param;

        character = param;

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.move.onTeleport += TeleportEvent;

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

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

        param.health.noLife -= ShowLoserWindow;

        //if (param.prin.itemBase != null)
        VirtualControllers.principal.DesuscribeController(param.prin);
        param.prin.updateTimer -= PrinUi;
        param.prin.finishTimer -= PrinUiFinish;

        //if (param.sec.itemBase != null)
        VirtualControllers.secondary.DesuscribeController(param.sec);
        param.sec.updateTimer -= SecUi;
        param.sec.finishTimer -= SecUiFinish;

        //if (param.ter.itemBase != null)
        VirtualControllers.terciary.DesuscribeController(param.ter);
        param.ter.updateTimer -= TerUi;
        param.ter.finishTimer -= TerUiFinish;

        VirtualControllers.movement.DesuscribeController(param.move);

        param.gameObject.tag = originalTag;
    }


    public override void OnStayState(Character param)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(EnumPlayer.move).Execute(transform.position);
    }

    private void SetJoystick(WeaponKata weaponKata, VirtualControllers.AxisButton axisButton, EnumController enumController, System.Action<float> ui, System.Action uifinish)
    {
        if (weaponKata != null)
        {
            axisButton.SuscribeController(weaponKata);
            weaponKata.updateTimer += ui;
            weaponKata.finishTimer += uifinish;
            EventManager.events.SearchOrCreate<EventJoystick>(enumController).ExecuteSet(true, weaponKata.itemBase.joystick);
        }
        else
        {
            EventManager.events.SearchOrCreate<EventJoystick>(enumController).ExecuteSet(false, false);
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