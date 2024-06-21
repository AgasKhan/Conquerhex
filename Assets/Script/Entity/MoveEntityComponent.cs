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

    public float desaceleration { get => move.desaceleration; set => move.desaceleration = value; }

    public Vector3 VectorVelocity { get => ((IMove)move).VectorVelocity; set => ((IMove)move).VectorVelocity = value; }

    public float objectiveVelocity { get => ((IMove)move).objectiveVelocity; set => ((IMove)move).objectiveVelocity = value; }
    public Vector3 VelocityCalculate { get => ((IMove)move).VelocityCalculate; set => ((IMove)move).VelocityCalculate = value; }
    public Vector3 direction { get => ((IMove)move).direction; set => ((IMove)move).direction = value; }

    Vector3 dirInput;

    bool flagZero;

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

    private void LateUpdate()
    {
        if(flagZero)
            dirInput.SetZero();

        flagZero = true;
    }

    private void FixedUpdate()
    {
        move.Acelerator(dirInput, move.aceleration.total);
        
    }

    public virtual void ControllerDown(Vector2 dir, float tim)
    {
    }

    public virtual void ControllerPressed(Vector2 dir, float tim)
    {
        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        if (dir.sqrMagnitude > 0)
        {
            dirInput = dir.Vect2To3XZ(0);
            flagZero = false;
        }
        else
        {
            dirInput.SetZero();
        }

        //velocity += aceleration.current * Time.deltaTime;
    }

    public virtual void ControllerUp(Vector2 dir, float tim)
    {
    }

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);
    }


    public void Teleport(Hexagone hex, int lado)
    {
        move.Teleport(hex, lado);
    }
    
}

