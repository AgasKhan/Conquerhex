using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    [SerializeField]
    protected MoveAbstract move;

    protected Vector2 _desiredVelocity;
    protected Vector2 _steering;

    protected Vector2 CalculateSteering(Vector2 target)
    {
        //_desiredVelocity = Direction(target).normalized * _maxSpeed;

        //_steering = _desiredVelocity - _velocity;

        //return AddVelocity(_steering);

        return Vector2.ClampMagnitude((target.normalized * move.maxSpeed) - move.vectorVelocity, move.aceleration.current);
    }

    protected void Arrive(Vector3 target)
    {
        _desiredVelocity = Vector2.ClampMagnitude(target, move.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (move.velocity * move.velocity / (move._desaceleration.current * move._desaceleration.current)))
            _desiredVelocity = -move.vectorVelocity * (move._desaceleration.current - 1);

        _steering = _desiredVelocity - move.vectorVelocity;

        var vecVelocity = move.vectorVelocity;

        vecVelocity += _steering * Time.deltaTime;

        move.Velocity(vecVelocity);
    }

    protected Vector2 DirectionSeek(MoveAbstract targetPos)
    {
        return Direction(targetPos.transform.position, 1);
    }

    protected Vector2 DirectionFlee(MoveAbstract targetPos)
    {
        return Direction(targetPos.transform.position, -1);
    }

    protected Vector2 DirectionPursuit(MoveAbstract targetPos)
    {
        //Calculo la distancia hacia el objetivo
        var ourDir = Direction(targetPos.transform.position, 1);

        //Saco la distancia según dónde estará el enemigo que va a una determinada velocidad
        Vector2 _directionToTarget = (targetPos.transform.position).Vect3To2() + targetPos.vectorVelocity - transform.position.Vect3To2();

        //Realizo una proyección del vector entre el punto donde estará el enemigo y nuestra posición actual
        Vector2 aux = Vector3.Project(_directionToTarget, ourDir);

        //Sumo el vector posición nuestra junto a la posición futura del enemigo y le resto la proyección para interceptarlo en caso de estar cerca o gire bruscamente
        Vector2 _directionToGo = ourDir + _directionToTarget - aux;


        //Vector2 agentToTarget = _directionToGo - (Vector2)transform.position; 

        ////Si la distancia al objetivo es menor a la distancia al punto del Pursuit podemos usar Seek
        //if (agentToTarget.sqrMagnitude <= targetPos.vectorVelocity.sqrMagnitude)
        //{
        //    return ourDir;
        //}

        return _directionToGo;
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

}

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


