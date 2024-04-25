using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveAbstract : MyScripts , IMove, IDamageable
{
    [field: SerializeField]
    public Vector3 direction { get; set; }

    public Tim aceleration = new Tim();

    public Tim _desaceleration = new Tim();

    public event System.Action<Vector3> onMove;

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

    public virtual Vector3 vectorVelocity 
    {
        get => velocity * direction.normalized;
        set
        {
            Velocity(value);
        }
    }

    protected void Set(Tim tim, float number)
    {
        tim.current = number;
    }

    public virtual MoveAbstract Acelerator(Vector3 dirNormalized, float magnitud, float? objectiveVelocity = null)
    {
        if (objectiveVelocity == null)
            objectiveVelocity = this.objectiveVelocity;

        Vector3 vecVelocity = velocity * direction.normalized;

        Vector3 calc;

        if (velocity > (float)objectiveVelocity)
        {
            calc = Vector3.ClampMagnitude(((float)objectiveVelocity * dirNormalized) - vecVelocity, desaceleration*Time.deltaTime);
        }
        else
        {
            calc = Vector3.ClampMagnitude(((float)objectiveVelocity * dirNormalized) - vecVelocity, magnitud * Time.deltaTime);
        }
        //vecVelocity = Vector3.ClampMagnitude((Time.deltaTime * magnitud * dirNormalized) + vecVelocity, (float)objectiveVelocity);

        //Debug.Log($"{velocity} {direction} {vectorVelocity} - {vecVelocity.magnitude}");

        vecVelocity += calc;

        aceleration.current = magnitud;

        velocity = vecVelocity.magnitude;

        direction = vecVelocity.normalized;

        return this;
    }

    public void Velocity(Vector3 dir, float? velocity=null)
    {
        this.velocity = velocity?? objectiveVelocity;

        this.direction = Vector3.ClampMagnitude(dir, 1);
    }

    public void Teleport(Hexagone hexagone, int lado)
    {
        onTeleport?.Invoke(hexagone, lado);
        transform.SetParent(hexagone.transform);
    }

    protected void OnIdle()
    {
        onIdle?.Invoke();
    }

    protected void OnMove(Vector3 vec)
    {
        onMove?.Invoke(vec);
    }

    public void InternalTakeDamage(ref Damage dmg, Vector3? damageOrigin = null)
    {
        if (damageOrigin == null || dmg.amount <= 0 || dmg.knockBack <= 0)
            return;

        Vector3 posDmg = (Vector3)damageOrigin;

        //Velocity((transform.position - posDmg).normalized * Mathf.Sign(dmg.knockBack), Mathf.Abs(dmg.knockBack) + velocity);

        var resultado = ((velocity * direction.normalized) + (transform.position - posDmg).normalized * dmg.knockBack);

        velocity = resultado.magnitude;

        direction = resultado.normalized;
    }
}

public interface IMove
{
    public event System.Action<Vector3> onMove;

    public event System.Action onIdle;

    /// <summary>
    /// primer parametro hexagono, segundo es el lado del cual se le teletransporta
    /// </summary>
    public event System.Action<Hexagone, int> onTeleport;

    public Vector3 direction { get; set; }

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
    public Vector3 vectorVelocity { get; set; }



    /// <summary>
    /// Funcion destinada a setear de forma manual la direccion de la velocidad y su magnitud
    /// </summary>
    /// <param name="dir">debe de estar normalizada para NO romperse</param>
    /// <param name="velocity">Por defecto sera la velocidad objetivo del componente</param>
    public void Velocity(Vector3 dir, float? velocity = null);
}