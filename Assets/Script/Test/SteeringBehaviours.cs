using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviour { Seek, Flee };
public class SteeringBehaviours : MonoBehaviour
{
    [SerializeField]
    Vector2Quad _obj;

    [SerializeField]
    float _maxSpeed = 7;

    [SerializeField]
    float _desaceleration = 1f;

    Vector2 _desiredVelocity;
    Vector2 _steering ;
    Vector2 _velocity = Vector2.zero;

    public Behaviour desiredBehav = Behaviour.Seek;

    public void Arrive()
    {
        _desiredVelocity = Vector2.ClampMagnitude(Direction(), _maxSpeed);

        if (_desiredVelocity.sqrMagnitude < _velocity.sqrMagnitude / (_desaceleration* _desaceleration))
            _desiredVelocity = -_velocity*(_desaceleration-1);

        _steering = _desiredVelocity - _velocity;

        AddVelocity(_steering);
    }

    public void Seek(float mukltiply)
    {
        _desiredVelocity = Direction(mukltiply).normalized * _maxSpeed;

        _steering = _desiredVelocity - _velocity;

        AddVelocity(_steering);
    }

    Vector2 Direction(float mukltiply=1)
    {
        Vector2 _direction = (_obj.tr.position - transform.position).Vect3To2();

        _direction *= mukltiply;

        return _direction;
    }

    void AddVelocity(Vector2 velocity)
    {
        _velocity += velocity * Time.deltaTime;//sumo la velocidad en metros por segundo
    }

    private void Locomotion()
    {
        transform.position += (_velocity * Time.deltaTime).Vec2to3(0);//me muevo en metros por segundo
    }


    /*
    void Manolo(float mukltiply)
    {
        Vector2 _direction = (_obj.tr.position - transform.position).Vect3To2() + _obj.velocity;

        _direction *= mukltiply;

        _steering = _direction - _velocity;

        if (_steering.sqrMagnitude> _aceleration * _aceleration)
            _steering = Vector2.ClampMagnitude(_steering, _aceleration);

        _velocity += _steering;
    }


    private void Move()
    {
        if (_velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            _velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);

        transform.position += (_velocity * Time.deltaTime).Vec2to3(0);
    }
    */

    private void Update()
    {
        Arrive();

        Locomotion();

        _obj.LoadVelocity();
    }

}
