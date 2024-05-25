using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    protected MoveAbstract me;

    protected Vector3 _desiredVelocity;
    protected Vector3 _steering;
    protected Vector3 _direction;

    public virtual SteeringBehaviour SwitchSteering<T>() where T : SteeringBehaviour
    {
        if (this is T)
            return this;
        
        if(!TryGetComponent<T>(out var aux))
        {
            aux = gameObject.AddComponent<T>();
            aux.me = me;
        }

        return aux;
    }

    protected virtual Vector3 Direction(MoveAbstract target,float multiply = 1)
    {

        _direction = (target.transform.position - me.transform.position);

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

    protected abstract Vector3 InternalCalculate(MoveAbstract target);
    
    public Vector3 Calculate(MoveAbstract target)
    {
        return InternalCalculate(target);
    }

    protected virtual void Awake()
    {
        me = GetComponent<MoveAbstract>();
    }
}




