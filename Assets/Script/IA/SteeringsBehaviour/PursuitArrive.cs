using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prototipo q combina ambas aun no funciona claramente
/// Tendria que aplicar encuentro con velocidad y aceleraciones variables
/// </summary>
public class PursuitArrive : SteeringBehaviour
{
    protected override Vector3 InternalCalculate(MoveAbstract target)
    {

        Vector3 desired = Direction(target);
        var speed = me.maxSpeed;

        _desiredVelocity = Vector3.ClampMagnitude(desired, me.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (me.VelocityCalculate.sqrMagnitude / (me._desaceleration.current * me._desaceleration.current)))
            _desiredVelocity = -me.VectorVelocity * (me._desaceleration.current - 1);

        _steering = _desiredVelocity - me.VectorVelocity;

        //Saco la distancia seg�n d�nde estar� el enemigo que va a una determinada velocidad
        Vector3 _directionToTarget = (target.transform.position) + target.VectorVelocity - me.transform.position;

        //Realizo una proyecci�n del vector entre el punto donde estar� el enemigo y nuestra posici�n actual
        Vector3 aux = Vector3.Project(_directionToTarget, desired);

        //Sumo el vector posici�n nuestra junto a la posici�n futura del enemigo y le resto la proyecci�n para interceptarlo en caso de estar cerca o gire bruscamente
        _steering+=  _directionToTarget - aux;



        return _steering;
    }
}
