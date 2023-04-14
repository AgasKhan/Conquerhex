using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    [SerializeField]
    protected float _maxSpeed = 7;

    [SerializeField]
    protected float _desaceleration = 1f;

    protected Vector2 _desiredVelocity;
    protected Vector2 _steering;
    protected Vector2 _velocity = Vector3.zero;

    //Vector2 _velocity;
    //public Vector2 velocity { get { return _velocity; } }

    //public virtual void Steering(Vector3 target)
    //{
    //    Vector3 _direction = (target - transform.position).Vect3To2();
    //    _direction *= 1;
    //    _desiredVelocity = _direction.normalized * _maxSpeed;

    //    _steering = _desiredVelocity - _velocity;

    //    _velocity += _steering * Time.deltaTime;

    //    transform.position += _velocity * Time.deltaTime;

    //}

    protected virtual Vector3 Seeking(Vector2 target)
    {
        _desiredVelocity = Direction(target).normalized * _maxSpeed;

        _steering = _desiredVelocity - _velocity;

        return AddVelocity(_steering);
    }

    Vector2 Direction(Vector3 targetPos, float multiply = 1)
    {

        Vector2 _direction = (targetPos - transform.position).Vect3To2();

        _direction *= multiply;

        return _direction;
    }

   public Vector2 AddVelocity(Vector2 velocity)
    {
        return _velocity += velocity * Time.deltaTime;//sumo la velocidad en metros por segundo
    }

    public void Locomotion()
    {
        transform.position += (_velocity * Time.deltaTime).Vec2to3(0);//me muevo en metros por segundo
    }

    public virtual void Arrive(Vector3 target)
    {
        _desiredVelocity = Vector2.ClampMagnitude(target, _maxSpeed);

        if (_desiredVelocity.sqrMagnitude < _velocity.sqrMagnitude / (_desaceleration * _desaceleration))
            _desiredVelocity = -_velocity * (_desaceleration - 1);

        _steering = _desiredVelocity - _velocity;

        _velocity += _steering * Time.deltaTime;
    }

    protected virtual Vector2 DirectionEvade(Vector3 targetPos)
    {
        return Direction(targetPos, -1);
    }

    protected virtual Vector2 DirectionPursuit(Vector3 targetPos)
    {
        return Direction(targetPos, 1);
    }


}

public class Pursuit : Seek
{
    //Transform pos;
    protected override Vector2 DirectionPursuit(Vector3 targetPos)
    {
        var ourDir = base.DirectionPursuit(targetPos);

       // var tarPos = base.DirectionPursuit(pos.position);


        //Vector2 _direction = Vector3.Project(tarPos + base.AddVelocity(targetPos), base.AddVelocity(targetPos)).Vect3To2();
        Vector2 _direction = Vector3.Project(ourDir + base.AddVelocity(targetPos), transform.position).Vect3To2();
        Vector2 _directionToGo = ourDir - _direction;

        Vector2 agentToTarget = _direction - (Vector2)transform.position; //_pursuitTarget.transform.position - transform.position;


        //Si la distancia al agente es < distancia al punto del Pursuit podemos usar Seek

        if (agentToTarget.sqrMagnitude <= base.AddVelocity(targetPos).sqrMagnitude)
        {
            return base.Seeking(targetPos);
        }

        return _directionToGo;
    }

    //public override void Steering(Vector3 target)
    //{

    //    Vector3 _direction = Vector3.Project(target + _velocity, transform.position).Vect3To2();
    //    _direction *= 1;
    //    _desiredVelocity = _direction.normalized * _maxSpeed;


    //    _steering = _desiredVelocity - _velocity;

    //    _velocity += _steering * Time.deltaTime;

    //    transform.position += _velocity * Time.deltaTime;
    //}


}


