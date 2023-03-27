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
    float _aceleration = 1f;

    Vector2 _desiredVelocity;
    Vector2 _steering ;
    Vector2 _velocity = Vector2.zero;

    

    //System.Action onStay;

    public Behaviour desiredBehav = Behaviour.Seek;
    public void Seek()
    {
        Manolo(1);
    }

    public void Flee()
    {
        Manolo(-1);
    }

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

    private void Update()
    {
        Seek();

        Move();

        _obj.LoadVelocity();
    }

}
