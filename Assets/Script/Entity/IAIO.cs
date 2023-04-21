using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAIO : MonoBehaviour, IState<Character>
{
    Character character;

    public void OnEnterState(Character param)
    {
        character = param;

        VirtualControllers.principal.SuscribeController(param.prin);
        VirtualControllers.secondary.SuscribeController(param.sec);
        VirtualControllers.terciary.SuscribeController(param.ter);

        VirtualControllers.movement.eventDown += Movement_eventDown;
        VirtualControllers.movement.eventPress += Movement_eventPress;
        VirtualControllers.movement.eventUp += Movement_eventUp;
    }

    private void Movement_eventDown(Vector2 arg1, float arg2)
    {
       
    }

    private void Movement_eventPress(Vector2 arg1, float arg2)
    {
        character.move.Velocity(arg1);
    }

    private void Movement_eventUp(Vector2 arg1, float arg2)
    {
    }

    public void OnExitState(Character param)
    {
    }

    public void OnStayState(Character param)
    {
        
    }

}
