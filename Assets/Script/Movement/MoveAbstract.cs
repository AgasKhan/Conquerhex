using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveAbstract : MyScripts, IControllerDir
{
    public GameObject carlitosPrefab;

    public Transform[] carlitos;

    public Vector2 direction;

    public Tim aceleration = new Tim();

    public Tim _desaceleration = new Tim();

    [SerializeField]
    protected Tim _velocity = new Tim();

    /// <summary>
    /// primer parametro hexagono, segundo es el lado del cual se le teletransporta
    /// </summary>
    public event System.Action<Hexagone, int> onTeleport;

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

    public virtual Vector2 vectorVelocity => velocity * direction;

    protected void Set(Tim tim, float number)
    {
        //tim.Substract(-(number- tim.current));

        tim.current = number;
    }

    public virtual MoveAbstract Acelerator(Vector2 dir)
    {
        if (velocity > aceleration.current)
            return this;

        var vecVelocity = vectorVelocity;

        vecVelocity += Vector2.ClampMagnitude(Time.deltaTime * dir + vecVelocity, aceleration.current) - vecVelocity;

        aceleration.current = dir.magnitude;

        velocity = vecVelocity.magnitude;

        direction = vecVelocity.normalized;

        return this;
    }

    /*
    public virtual MoveAbstract Acelerator(Vector2 dir)
    {
        var vecVelocity = vectorVelocity;

        vecVelocity += dir;

        velocity = vecVelocity.magnitude;

        direction = vecVelocity.normalized;

        return this;
    }
    */

    public virtual MoveAbstract Velocity(Vector2 dir)
    {
        velocity = dir.magnitude;

        direction = dir.normalized;

        return this;
    }

    public void Teleport(Hexagone hexagone, int lado)
    {
        onTeleport?.Invoke(hexagone, lado);
    }

    public virtual void ControllerDown(Vector2 dir, float tim)
    {
        //_desaceleration.current = 0;
    }

    public virtual void ControllerPressed(Vector2 dir, float tim)
    {
        var aux = dir * _velocity.total;

        if(aux.sqrMagnitude>0)
            Acelerator(aux);
        //velocity += aceleration.current * Time.deltaTime;
    }

    public virtual void ControllerUp(Vector2 dir, float tim)
    {
        //_desaceleration.Reset();
    }
}