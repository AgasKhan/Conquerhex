using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{

    string originalTag;

    public override void OnEnterState(Character param)
    {
        character = param;

        originalTag = param.gameObject.tag;

        param.gameObject.tag = "Player";

        param.health.lifeUpdate += UpdateLife;
        param.health.regenUpdate += UpdateRegen;


        if (param.prin.itemBase !=null)
        {
            VirtualControllers.principal.SuscribeController(param.prin);
            param.prin.updateTimer += PrinUi;
        }
            

        if (param.sec.itemBase != null)
        {
            VirtualControllers.secondary.SuscribeController(param.sec);
            param.sec.updateTimer += SecUi;
        }
            

        if (param.ter.itemBase != null)
        {
            VirtualControllers.terciary.SuscribeController(param.ter);
            param.ter.updateTimer += TerUi;
        }
            


        VirtualControllers.movement.SuscribeController(param.move);
    }



    public override void OnExitState(Character param)
    {
        param.health.lifeUpdate -= UpdateLife;
        param.health.regenUpdate -= UpdateRegen;

        //if (param.prin.itemBase != null)
        VirtualControllers.principal.DesuscribeController(param.prin);
        param.prin.updateTimer -= PrinUi;

        //if (param.sec.itemBase != null)
        VirtualControllers.secondary.DesuscribeController(param.sec);
        param.sec.updateTimer -= SecUi;

        //if (param.ter.itemBase != null)
        VirtualControllers.terciary.DesuscribeController(param.ter);
        param.ter.updateTimer -= TerUi;


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

    void UpdateLife(IGetPercentage getPercentage)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).Execute(getPercentage);
    }

    void UpdateRegen(IGetPercentage getPercentage)
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.regen).Execute(getPercentage);
    }

}
