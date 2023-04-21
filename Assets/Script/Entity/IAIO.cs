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
        VirtualControllers.movement.SuscribeController(param.move);
    }



    public void OnExitState(Character param)
    {
    }

    public void OnStayState(Character param)
    {
        
    }

}
