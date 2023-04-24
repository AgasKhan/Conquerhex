using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : MonoBehaviour, IState<Character>
{
    Character character;

    [SerializeField]
    SteeringWithTarger[] steerings;

    private void Update()
    {
        foreach (var steer in steerings)
        {
            foreach (var obj in steer.targets)
            {
                steer.steering.Calculate(obj);
            }
        }
    }

    public void OnEnterState(Character param)
    {
        character = param;

        VirtualControllers.principal.SuscribeController(param.prin);
        VirtualControllers.secondary.SuscribeController(param.sec);
        VirtualControllers.terciary.SuscribeController(param.ter);
        VirtualControllers.movement.SuscribeController(param.move);
    }



    public void OnExitState(Character param)
    {
        VirtualControllers.principal.DesuscribeController(param.prin);
        VirtualControllers.secondary.DesuscribeController(param.sec);
        VirtualControllers.terciary.DesuscribeController(param.ter);
        VirtualControllers.movement.DesuscribeController(param.move);
    }

    public void OnStayState(Character param)
    {
        
    }
}

[System.Serializable]
struct SteeringWithTarger
{
    public SteeringBehaviour steering;

    public List<MoveAbstract> targets;
}