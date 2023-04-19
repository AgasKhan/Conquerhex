using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveAbstract : MonoBehaviour
{
    public Vector2 direction;

    public Tim aceleration;

    public Tim _desaceleration;

    [SerializeField]
    protected Tim _velocity;

    [SerializeField]
    protected float _frameVelocity;

    [SerializeField]
    protected float _frameDesaceleration;

    public float velocity
    {
        get => _velocity.current;
        set
        {
            Set(_velocity, value);
            _frameVelocity = _velocity.current * Time.deltaTime;
        }
    }

    public float desaceleration
    {
        get => _desaceleration.current;
        set
        {
            Set(_desaceleration, value);
            _frameDesaceleration = _desaceleration.current * Time.deltaTime;
        }
    }

    protected Vector2 ToVector(Tim tim)
    {
        return direction * tim.current;
    }

    protected void Set(Tim tim, float number)
    {
        tim.Substract(-(number- tim.current));
    }

    public virtual MoveAbstract Acelerator(Vector2 dir)
    {
        var vecVelocity = ToVector(_velocity);

        vecVelocity += aceleration.current * Time.deltaTime * dir;

        velocity = vecVelocity.magnitude;

        direction = vecVelocity.normalized;

        return this;
    }

    public virtual MoveAbstract Acelerator(float number)
    {
        Set(aceleration, number);

        return this;
    }

    public virtual MoveAbstract Velocity(Vector2 dir)
    {
        velocity = dir.magnitude;

        direction = dir.normalized;

        return this;
    }

    public virtual MoveAbstract Velocity(float number)
    {
        velocity = number;

        return this;
    }

    public virtual MoveAbstract Desacelerator(float number)
    {
        desaceleration = number;

        return this;
    }
}