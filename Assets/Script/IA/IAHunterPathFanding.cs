using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunterPathFanding : IAHunter
{
    Stack<Transform> nodes = new Stack<Transform>();

    Transform lastPositionPlayer;

    public override Transform currentObjective
    {
        get
        {

            if (nodes.Count>0)
            {
                var node = nodes.Peek();

                if ((node.position - transform.position).sqrMagnitude < minimalDistance * minimalDistance)
                {
                    nodes.Pop();
                    if (!nodes.TryPeek(out node))
                    {
                        return patrol.currentWaypoint;
                    }
                }
                return node;
            }

            return patrol.currentWaypoint;
        }
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        Pathfinding.instance.newObjective += NewObjectiveEvent;

        fsm.detectEnemy += Fsm_detectEnemy;

        fsm.noDetectEnemy += Fsm_noDetectEnemy;

        patrol.fsmPatrol.OnMove += OnPatrol;
    }

 
    public override void OnExitState(Character param)
    {
        base.OnExitState(param);

        Pathfinding.instance.newObjective -= NewObjectiveEvent;

        fsm.detectEnemy -= Fsm_detectEnemy;

        fsm.noDetectEnemy -= Fsm_noDetectEnemy;

        patrol.fsmPatrol.OnMove -= OnPatrol;
    }

    private void Fsm_noDetectEnemy(Vector3 obj)
    {
        if(lastPositionPlayer == null)
        {
            lastPositionPlayer = new GameObject("lastPosition Player").transform;
        }

        lastPositionPlayer.position = obj;
        
        Stack<Transform> aux = Pathfinding.instance.CalculatePath(obj, transform.position);

        nodes.Clear();

        nodes.Push(lastPositionPlayer);

        while (aux.TryPop(out var node))
        {
            nodes.Push(node);
        }
    }

    private void Fsm_detectEnemy(Vector3 obj)
    {
        Pathfinding.instance.NotifyNewObjective(obj);
    }

    private void NewObjectiveEvent(Vector3 obj)
    {
        nodes = Pathfinding.instance.CalculatePath(transform.position, obj);
    }

    void OnPatrol()
    {
        //var aux = detectCordero.RayTransform(transform.position, (currentObjective.position - transform.position), (cmp)=> { return cmp!=transform; },(currentObjective.position - transform.position).magnitude);

        var aux = Physics2D.RaycastAll(transform.position, (currentObjective.position - transform.position), NodeManager.instance.BlockedNodeLayer);

        if(aux!= null && aux.Length>1)
        {
            nodes = Pathfinding.instance.CalculatePath(transform.position, patrol.currentWaypoint.position);
        }
    }
}
