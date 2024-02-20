using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class MoveEntityComponent : ComponentOfContainer<Entity>
{
    public MoveAbstract move;

    public override void OnEnterState(Entity param)
    {
        container = param;
        move.onTeleport += param.Teleport;
    }

    public override void OnExitState(Entity param)
    {
        
    }

    public override void OnStayState(Entity param)
    {
        
    }
}

