using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : SteeringBehaviour
{


    protected override Vector2 InternalCalculate(MoveAbstract target)
    {
        //Calculo la distancia hacia el objetivo
        var ourDir = Direction(target);

        //Saco la distancia según dónde estará el enemigo que va a una determinada velocidad
        Vector2 _directionToTarget = (target.transform.position.Vect3To2() + target.vectorVelocity) - me.transform.position.Vect3To2();

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
}
