using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviour { Seek, Flee };
public class SteeringBehaviours : MonoBehaviour
{
    [SerializeField]
    Transform objective;

    [SerializeField]
    float _maxSpeed = 7;
    float _slowSpeed = 1f;

    Vector2 _desiredVelocity;
    Vector2 _steering;
    Vector2 _velocity = Vector2.zero;

    public Behaviour desiredBehav = Behaviour.Seek;
    public void SeekOrFlee()
    {
        Vector2 _direction = (objective.transform.position - transform.position);
        _direction = desiredBehav == Behaviour.Seek ? _direction : _direction * -1; 

        _desiredVelocity = _direction.normalized * _maxSpeed;
        _steering = _desiredVelocity - _velocity;
        _velocity += _steering * Time.deltaTime;

        //Vector3 _slowDown = Vector3.ClampMagnitude(_direction, _slowSpeed);

        float _slowDown = Mathf.Clamp01(_direction.magnitude / _slowSpeed);
        _velocity *= _slowDown;

        Move();

    }

    private void Move()
    {
        transform.position += (Vector3)_velocity * Time.deltaTime;
    }


    private void Update()
    {
        SeekOrFlee();
    }

}
