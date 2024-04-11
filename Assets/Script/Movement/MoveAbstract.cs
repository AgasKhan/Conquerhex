using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveAbstract : MyScripts , IMove
{
    [field: SerializeField]
    public Vector2 direction { get; set; }

    public Tim aceleration = new Tim();

    public Tim _desaceleration = new Tim();

    public event System.Action<Vector2> onMove;

    public event System.Action onIdle;

    public event Action<Hexagone, int> onTeleport;

    [SerializeField]
    protected Tim _velocity = new Tim();



    [field: SerializeField]
    public float objectiveVelocity { get; set; }

    public float maxSpeed
    {
        get => _velocity.total;
        set => _velocity.total = value;
    }

    public float velocity
    {
        get => _velocity.current;
        set
        {
            Set(_velocity, value);
            
        }
    }

    public float desaceleration
    {
        get => _desaceleration.current;
        set
        {
            Set(_desaceleration, value);
            
        }
    }

    public virtual Vector2 vectorVelocity 
    {
        get => velocity * direction;
        set
        {
            Velocity(value);
        }
    }

    protected void Set(Tim tim, float number)
    {
        tim.current = number;
    }

    public virtual MoveAbstract Acelerator(Vector2 dirNormalized, float magnitud, float? objectiveVelocity = null)
    {
        if (objectiveVelocity == null)
            objectiveVelocity = this.objectiveVelocity;

        /*
        if (velocity > objectiveVelocity)
            return this;
        */

        var vecVelocity = vectorVelocity;

        vecVelocity += Vector2.ClampMagnitude((Time.deltaTime * magnitud * dirNormalized) + vecVelocity, (float)objectiveVelocity) - vecVelocity;

        aceleration.current = magnitud;

        velocity = vecVelocity.magnitude;

        direction = vecVelocity.normalized;

        return this;
    }

    public void Velocity(Vector2 dir, float? velocity=null)
    {
        this.velocity = velocity?? objectiveVelocity;

        this.direction = dir;
    }

    MoveAbstract Velocity(Vector2 dir)
    {
        velocity = dir.magnitude;

        direction = dir.normalized;

        return this;
    }

    public void Teleport(Hexagone hexagone, int lado)
    {
        onTeleport?.Invoke(hexagone, lado);
    }


    protected void OnIdle()
    {
        onIdle?.Invoke();
    }

    protected void OnMove(Vector2 vec)
    {
        onMove?.Invoke(vec);
    }
}

public interface IMove
{
    public event System.Action<Vector2> onMove;

    public event System.Action onIdle;

    /// <summary>
    /// primer parametro hexagono, segundo es el lado del cual se le teletransporta
    /// </summary>
    public event System.Action<Hexagone, int> onTeleport;

    public Vector2 direction { get; set; }

    public float objectiveVelocity { get; set; }

    public float maxSpeed
    {
        get;
        set;
    }

    public float velocity
    {
        get;
        set;
    }

    public float desaceleration
    {
        get;
        set;
    }

    /// <summary>
    /// ATENCION: es mas lenta para setear que su contraparte manual Velocity(Vector2 dir, float velocity)<br/>
    /// Funcion destinada a setear de forma manual la direccion de la velocidad
    /// </summary>
    public Vector2 vectorVelocity { get; set; }



    /// <summary>
    /// Funcion destinada a setear de forma manual la direccion de la velocidad y su magnitud
    /// </summary>
    /// <param name="dir">debe de estar normalizada para NO romperse</param>
    /// <param name="velocity">Por defecto sera la velocidad objetivo del componente</param>
    public void Velocity(Vector2 dir, float? velocity = null);
}