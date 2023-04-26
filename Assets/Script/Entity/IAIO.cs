using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : IAFather
{
    Character character;

    [SerializeField]
    SteeringWithTarger[] steerings;

    private void Update()
    {
        /*
        foreach (var steer in steerings)
        {
            foreach (var obj in steer.targets)
            {
                steer.steering.Calculate(obj);
            }
        }
        */
    }

    public override void OnEnterState(Character param)
    {
        character = param;
        
        if(param.prin.itemBase !=null)
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

}

[System.Serializable]
struct SteeringWithTarger
{
    public SteeringBehaviour steering;

    public List<MoveAbstract> targets;
}