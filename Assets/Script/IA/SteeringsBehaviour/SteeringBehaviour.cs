using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    [SerializeField]
    protected MoveAbstract me;

    protected Vector2 _desiredVelocity;
    protected Vector2 _steering;

    public SteeringBehaviour SwitchSteering<T>() where T : SteeringBehaviour
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

    protected abstract Vector2 InternalCalculate(MoveAbstract target);
    
    public Vector2 Calculate(MoveAbstract target)
    {
        return InternalCalculate(target);
    }

    private void Awake()
    {
        me = GetComponent<MoveAbstract>();
    }
}




