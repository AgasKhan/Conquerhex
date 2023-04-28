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



/*
 
 
    protected Vector2 DirectionSeek(MoveAbstract targetPos)
    {
        
    }

    protected Vector2 DirectionFlee(MoveAbstract targetPos)
    {
        return Direction(targetPos.transform.position, -1);
    }

    protected Vector2 DirectionPursuit(MoveAbstract targetPos)
    {
        
    }

    protected Vector2 DirectionEvade (MoveAbstract targetPos)
    {
        return -DirectionPursuit(targetPos);
    }

    Vector2 Direction(Vector3 targetPos, float multiply = 1)
    {

        Vector2 _direction = (targetPos - transform.position).Vect3To2();

        _direction *= multiply;

        return _direction;
    }
 
 
 
 
 
 
 
 */










//public class Pursuit : Seek
//{
//    //Transform pos;
//    public override Vector2 DirectionPursuit(MoveAbstract targetPos)
//    {
//        var ourDir = base.DirectionPursuit(targetPos);

//       // var tarPos = base.DirectionPursuit(pos.position);


//        //Vector2 _direction = Vector3.Project(tarPos + base.AddVelocity(targetPos), base.AddVelocity(targetPos)).Vect3To2();
//        Vector2 _direction = Vector3.Project((Vector2)targetPos.transform.position + targetPos.vectorVelocity, transform.position).Vect3To2();
//        Vector2 _directionToGo = ourDir - _direction;


//        Vector2 agentToTarget = _direction - (Vector2)transform.position; //_pursuitTarget.transform.position - transform.position;

//        //Si la distancia al agente es < distancia al punto del Pursuit podemos usar Seek
//        if (agentToTarget.sqrMagnitude <= targetPos.vectorVelocity.sqrMagnitude)
//        {
//            return ourDir;
//        }

//        return _directionToGo;
//    }

//public override void Steering(Vector3 target)
//{

//    Vector3 _direction = Vector3.Project(target + _velocity, transform.position).Vect3To2();
//    _direction *= 1;
//    _desiredVelocity = _direction.normalized * _maxSpeed;


//    _steering = _desiredVelocity - _velocity;

//    _velocity += _steering * Time.deltaTime;

//    transform.position += _velocity * Time.deltaTime;
//}


//}


