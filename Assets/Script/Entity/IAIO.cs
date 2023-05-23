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


        if (param.prin !=null)
        {
            VirtualControllers.principal.SuscribeController(param.prin);
            param.prin.updateTimer += PrinUi;
            param.prin.finishTimer += PrinUiFinish;
        }
            

        if (param.sec != null)
        {
            VirtualControllers.secondary.SuscribeController(param.sec);
            param.sec.updateTimer += SecUi;
            param.sec.finishTimer += SecUiFinish;
        }
            

        if (param.ter != null)
        {
            VirtualControllers.terciary.SuscribeController(param.ter);
            param.ter.updateTimer += TerUi;
            param.ter.finishTimer += TerUiFinish;
        }
            
        VirtualControllers.movement.SuscribeController(param.move);
    }

    private void TeleportEvent(Hexagone obj, int lado)
    {
        obj.SetRenders(HexagonsManager.LadoOpuesto(lado));
    }

    public override void OnExitState(Character param)
    {
        param.move.onTeleport -= TeleportEvent;

        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

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
        
    }

    void PrinUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(ControllerEnum.principal).Execute(f);
    }

    void SecUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(ControllerEnum.secondary).Execute(f);
    }

    void TerUi(float f)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(ControllerEnum.terciary).Execute(f);
    }

    void PrinUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(ControllerEnum.principal).ExecuteEnd();
    }

    void SecUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(ControllerEnum.secondary).ExecuteEnd();
    }

    void TerUiFinish()
    {
        EventManager.events.SearchOrCreate<EventTimer>(ControllerEnum.terciary).ExecuteEnd();
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
