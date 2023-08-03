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

    protected override void Awake()
    {
        base.Awake();

        pathFinding = new PathFinding(transform);
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        PathfindingManager.instance.newObjective += pathFinding.GoTo;

        patrol.OnPatrolChange += pathFinding.GoTo;

        fsm.detectEnemy += Fsm_detectEnemy;

        move.onMove += Move_onMove;
    }

    public override void OnExitState(Character param)
    {
        base.OnExitState(param);

        PathfindingManager.instance.newObjective -= pathFinding.GoTo;

        patrol.OnPatrolChange -= pathFinding.GoTo;

        fsm.detectEnemy -= Fsm_detectEnemy;

        move.onMove -= Move_onMove;
    }

    private void Fsm_detectEnemy(Vector3 obj)
    {
        PathfindingManager.instance.NotifyNewObjective(obj);
    }

    private void Move_onMove(Vector2 obj)
    {
        pathFinding.ViewOfTarget();
    }
}
