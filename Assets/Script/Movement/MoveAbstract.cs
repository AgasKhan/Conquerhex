using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveAbstract : MyScripts , IMove, IDamageable
{
    [field: SerializeField]
    public Vector3 direction { get; set; }

    [SerializeField]
    Vector3 _velocityCalculate;

    [field: SerializeField]
    public float objectiveVelocity { get; set; } = 7.5f;

    [field: SerializeField]
    public float maxSpeed { get; set; } = 100;

    public Tim aceleration = new Tim();

    public Tim _desaceleration = new Tim();

    public event System.Action<Vector3> onMove;

    public event System.Action onIdle;

    public event Action<Hexagone, int> onTeleport;

    public float desaceleration
    {
        get => _desaceleration.current;
        set
        {
            Set(_desaceleration, value);
        }
    }

    public virtual Vector3 VectorVelocity
    {
        get => VelocityCalculate;
        set
        {
            Velocity(value);
        }
    }

    public Vector3 VelocityCalculate 
    {
        get => _velocityCalculate;
        set
        {
            _velocityCalculate = Vector3.ClampMagnitude(value, maxSpeed);
            if (_velocityCalculate.sqrMagnitude < 0.1f)
                _velocityCalculate = Vector3.zero;
            else
                direction = _velocityCalculate.normalized;
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

        Vector3 calc;

        if (VelocityCalculate.sqrMagnitude > (float)objectiveVelocity * (float)objectiveVelocity)
        {
            calc = Vector3.ClampMagnitude(((float)objectiveVelocity * dirNormalized) - VelocityCalculate, desaceleration*Time.deltaTime);
        }
        else
        {
            calc = Vector3.ClampMagnitude(((float)objectiveVelocity * dirNormalized) - VelocityCalculate, magnitud * Time.deltaTime);
        }
        //vecVelocity = Vector3.ClampMagnitude((Time.deltaTime * magnitud * dirNormalized) + vecVelocity, (float)objectiveVelocity);

        //Debug.Log($"{velocity} {direction} {vectorVelocity} - {vecVelocity.magnitude}");

        VelocityCalculate += calc;

        aceleration.current = magnitud;

        return this;
    }

    public void Velocity(Vector3 dir, float? velocity=null)
    {
        VelocityCalculate = Vector3.ClampMagnitude(dir, 1) * (velocity ?? objectiveVelocity);
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

    public void InternalTakeDamage(ref Damage dmg, int weightAction = 0 ,Vector3? damageOrigin = null)
    {
        if (damageOrigin == null || dmg.amount <= 0 || Mathf.Abs(dmg.knockBack) <= 0)
            return;

        Vector3 posDmg = (Vector3)damageOrigin;

        //Velocity((transform.position - posDmg).normalized * Mathf.Sign(dmg.knockBack), Mathf.Abs(dmg.knockBack) + velocity);

        VelocityCalculate += (transform.position - posDmg).normalized * dmg.knockBack;
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

    /// <summary>
    /// propiedad destinada a guardar de forma manual la velocidad calculada
    /// </summary>
    public Vector3 VelocityCalculate { get; set; }

    public float objectiveVelocity { get; set; }

    public float maxSpeed
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
    /// Funcion destinada a setear de forma manual la velocidad REAL
    /// </summary>
    public Vector3 VectorVelocity { get; set; }

    public Vector3 direction { get; set; }

    /// <summary>
    /// Funcion destinada a setear de forma manual la direccion de la velocidad y su magnitud
    /// </summary>
    /// <param name="dir">debe de estar normalizada para NO romperse</param>
    /// <param name="velocity">Por defecto sera la velocidad objetivo del componente</param>
    public void Velocity(Vector3 dir, float? velocity = null);
}