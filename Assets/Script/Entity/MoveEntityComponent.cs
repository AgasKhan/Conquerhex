using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
using System;

[RequireComponent(typeof(MoveAbstract))]
public class MoveEntityComponent : ComponentOfContainer<Entity>, IControllerDir, IMove, ISaveObject
{
    [SerializeField]
    MoveAbstract move;

    public float maxSpeed { get => move.maxSpeed; set => move.maxSpeed = value; }
    public float velocity { get => move.velocity; set => move.velocity = value; }
    public float desaceleration { get => move.desaceleration; set => move.desaceleration = value; }

    public Vector3 vectorVelocity { get => ((IMove)move).vectorVelocity; set => ((IMove)move).vectorVelocity = value; }
    public Vector3 direction { get => ((IMove)move).direction; set => ((IMove)move).direction = value; }
    public float objectiveVelocity { get => ((IMove)move).objectiveVelocity; set => ((IMove)move).objectiveVelocity = value; }

    public event Action<Vector3> onMove
    {
        add
        {
            ((IMove)move).onMove += value;
        }

        remove
        {
            ((IMove)move).onMove -= value;
        }
    }

    public event Action onIdle
    {
        add
        {
            ((IMove)move).onIdle += value;
        }

        remove
        {
            ((IMove)move).onIdle -= value;
        }
    }

    public event Action<Hexagone, int> onTeleport
    {
        add
        {
            ((IMove)move).onTeleport += value;
        }

        remove
        {
            ((IMove)move).onTeleport -= value;
        }
    }

    public void Velocity(Vector3 dir, float? velocity = null)
    {
        ((IMove)move).Velocity(dir, velocity);
    }


    public override void OnEnterState(Entity param)
    {
        move.onTeleport += param.Teleport;
    }

    public override void OnExitState(Entity param)
    {
        move.onTeleport -= param.Teleport;
    }

    public override void OnStayState(Entity param)
    {

    }

    

    public virtual void ControllerDown(Vector2 dir, float tim)
    {
        //_desaceleration.current = 0;
    }

    public virtual void ControllerPressed(Vector2 dir, float tim)
    {
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        if (dir.sqrMagnitude > 0)
        {
            move.Acelerator(dir.Vect2To3XZ(0), move.aceleration.total);
        }

        //velocity += aceleration.current * Time.deltaTime;
    }

    public virtual void ControllerUp(Vector2 dir, float tim)
    {
        //_desaceleration.Reset();
    }

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
    }

    
}

