using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunterPathFanding : IAHunter
{
    [SerializeReference]
    PathFinding pathFinding;

    public override Transform currentObjective
    {
        get
        {
            return pathFinding.currentObjective;
        }
    }


    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        pathFinding = new PathFinding(transform);

        patrol.OnPatrolChange += pathFinding.GoTo;

        param.move.onMove += Move_onMove;
    }

    public override void OnExitState(Character param)
    {
        base.OnExitState(param);

        PathfindingManager.instance.newObjective -= pathFinding.GoTo;

        patrol.OnPatrolChange -= pathFinding.GoTo;

        param.move.onMove -= Move_onMove;
    }


    private void Move_onMove(Vector3 obj)
    {
        pathFinding.ViewOfTarget();
    }
}
