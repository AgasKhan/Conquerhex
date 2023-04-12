using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : MonoBehaviour
{
    [SerializeField] float _maxSpeed;

    [Range(0, 0.1f)]
    [SerializeField] float _maxSeekForce;
    [SerializeField] float _maxPursuitForce;

    Vector3 _velocity;

    [SerializeField] RandomDir _pursuitTarget;

    void Update()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }

    Vector3 Seek(Vector3 target)
    {
        return target;
    }

    Vector3 Persuit()
    {
        //Calcular nuestro Desire en base al proximo frame del enemigo

        Vector3 futurePos = _pursuitTarget.transform.position + _pursuitTarget.velocity * 0.5f;

        Vector3 agentToTarget = _pursuitTarget.transform.position - transform.position;


        //Si la distancia al agente es < distancia al punto del Pursuit podemos usar Seek

        if(agentToTarget.sqrMagnitude <= _pursuitTarget.velocity.sqrMagnitude)
        {
            return Seek(_pursuitTarget.transform.position);
        }

        //

        Vector3 desired = futurePos - transform.position;

        desired.Normalize();

        desired *= _maxSpeed;

        //steering
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxPursuitForce);


        return Seek(futurePos);
    }

    Vector3 Evade()
    {
        return -Persuit();
    }


    void AddForce(Vector3 force)
    {
        _velocity += force;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

    }

}

public class RandomDir : MonoBehaviour
{
    [SerializeField] float _maxSpeed;

    [Range(0, 0.1f)]
    [SerializeField] float _maxForce;

    Vector3 _velocity;
    public Vector3 velocity { get { return _velocity; } }

    void Start()
    {
        Vector3 randomDit = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        AddForce(randomDit.normalized * _maxSpeed);
    }

    private void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.forward = _velocity;

        CheckBound();
    }

    void CheckBound()
    {
        Vector3 pos = transform.position;

        if (pos.x >= 9) pos.x = -9;
        else if (pos.x <= -9) pos.x = 9;

        if (pos.y >= 4) pos.y = -4;
        else if (pos.y <= -4) pos.y = 4;

        transform.position = pos;
    }

    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

    }
}
