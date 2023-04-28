using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    protected MoveAbstract me;

    protected Vector2 _desiredVelocity;
    protected Vector2 _steering;

    public SteeringBehaviour SwitchSteering<T>() where T : SteeringBehaviour
    {
        if(!TryGetComponent<T>(out var aux))
        {
            aux = gameObject.AddComponent<T>();
            aux.me = me;
        }
        return aux;
    }

    protected Vector2 CalculateSteering(Vector2 target)
    {
        //_desiredVelocity = Direction(target).normalized * _maxSpeed;

        //_steering = _desiredVelocity - _velocity;

        //return AddVelocity(_steering); SteeringBehaviour

        return Vector2.ClampMagnitude((target.normalized * me.maxSpeed) - me.vectorVelocity, me.aceleration.current);
    }


    protected virtual Vector2 Direction(MoveAbstract target,float multiply = 1)
    {

        Vector2 _direction = (target.transform.position - me.transform.position).Vect3To2();

        _direction *= multiply;

        return _direction;
    }

    /*
    public Vector2 Calculate(Transform target)
    {
        if (!TryGetComponent<MoveAbstract>(out var aux))
        {
            aux = target.gameObject.AddComponent<MoveAbstract>();
            aux.enabled = false;
        }

        return Calculate(aux);
    }
    */

    public abstract Vector2 Calculate(MoveAbstract target);

    private void Awake()
    {
        me = GetComponent<MoveAbstract>();
    }
}




