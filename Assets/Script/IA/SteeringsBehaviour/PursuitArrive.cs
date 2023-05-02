using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prototipo q combina ambas aun no funciona claramente
/// Tendria que aplicar encuentro con velocidad y aceleraciones variables
/// </summary>
public class PursuitArrive : SteeringBehaviour
{
    protected override Vector2 InternalCalculate(MoveAbstract target)
    {

        Vector2 desired = Direction(target);
        var speed = me.maxSpeed;

        _desiredVelocity = Vector2.ClampMagnitude(desired, me.maxSpeed);

        if (_desiredVelocity.sqrMagnitude < (me.velocity * me.velocity / (me._desaceleration.current * me._desaceleration.current)))
            _desiredVelocity = -me.vectorVelocity * (me._desaceleration.current - 1);

        _steering = _desiredVelocity - me.vectorVelocity;

        //Saco la distancia según dónde estará el enemigo que va a una determinada velocidad
        Vector2 _directionToTarget = (target.transform.position).Vect3To2() + target.vectorVelocity - me.transform.position.Vect3To2();

        //Realizo una proyección del vector entre el punto donde estará el enemigo y nuestra posición actual
        Vector2 aux = Vector3.Project(_directionToTarget, desired);

        //Sumo el vector posición nuestra junto a la posición futura del enemigo y le resto la proyección para interceptarlo en caso de estar cerca o gire bruscamente
        _steering+=  _directionToTarget - aux;



        return _steering;
    }
}
