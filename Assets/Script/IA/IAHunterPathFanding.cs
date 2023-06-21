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
            if(!nodes.TryPeek(out var nodo))
            {
                return patrol.currentWaypoint;
            }
            if ((nodo.transform.position - transform.position).sqrMagnitude < minimalDistance * minimalDistance)
                if (nodes.TryPop(out nodo) && !nodes.TryPeek(out nodo))
                {
                    return patrol.currentWaypoint;
                }

            return nodo.transform;
        }
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        Pathfinding.instance.newObjective += NewObjectiveEvent;

        fsm.detectEnemy += Fsm_detectEnemy;

        fsm.noDetectEnemy += Fsm_noDetectEnemy;

        patrol.fsmPatrol.OnStartMove += OnStartPatrol;
    }

 
    public override void OnExitState(Character param)
    {
        base.OnExitState(param);

        Pathfinding.instance.newObjective -= NewObjectiveEvent;

        fsm.detectEnemy -= Fsm_detectEnemy;

        patrol.fsmPatrol.OnStartMove -= OnStartPatrol;
    }

    private void Fsm_noDetectEnemy(Vector3 obj)
    {
        if(lastPositionPlayer == null)
        {
            lastPositionPlayer = new GameObject("lastPosition Player").transform;
        }

        lastPositionPlayer.transform.position = obj;

        nodes.Push(lastPositionPlayer.transform);

        foreach (var item in Pathfinding.instance.CalculatePath(transform.position, obj))
        {
            nodes.Push(item);
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
    void OnStartPatrol()
    {
        var aux = detectCordero.RayTransform(transform.position, (currentObjective.position - transform.position), (currentObjective.position - transform.position).magnitude);

        if(aux!= null && aux.Length>1)
        {
            nodes = Pathfinding.instance.CalculatePath(transform.position, currentObjective.position);
        }
    }
}
