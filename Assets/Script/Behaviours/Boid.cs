using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : Seek
{

    delegate void _FuncBoid(ref Vector2 desired, Boid objective, Vector2 dirToBoid);

    void Start()
    {
        Manager<Boid>.pic.Add(GetInstanceID().ToString(), this);

        Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        move.Velocity(move.maxSpeed).Velocity(random.normalized);
    }

    void Update()
    {
        move.Acelerator(BoidIntern(Separation, false) * BoidsManager.instance.SeparationWeight +
                 BoidIntern(Alignment, true) * BoidsManager.instance.AlignmentWeight +
                 BoidIntern(Cohesion, true) * BoidsManager.instance.CohesionWeight);

        CheckBounds();
    }

    void CheckBounds()
    {
        transform.position = Boundaries.instance.SetObjectBoundPosition(transform.position);
    }

    

    Vector2 BoidIntern(_FuncBoid func, bool promedio)
    {
        Vector2 desired = Vector2.zero;

        int count = 0;

        //Por cada boid
        foreach (var boid in Manager<Boid>.pic)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid.value == this) continue;

            //Saco la direccion hacia el boid
            Vector2 dirToBoid = DirectionSeek(boid.value.move);

            //Si esta dentro del rango de vision, seteo un func que variará según el movimiento que se desea
            if (dirToBoid.sqrMagnitude <= BoidsManager.instance.SeparationRadius)
            {
                func(ref desired, boid.value, dirToBoid);

                count++;
            }
        }

        if (desired == Vector2.zero) return desired;

        //En caso de requerir tener el promedio de todos los boids, promedio con mi desired
        if(promedio)
            desired /= count;

        return CalculateSteering(desired);
    }

    void Separation(ref Vector2 desired, Boid boid, Vector2 dirToBoid)
    {
        desired -= dirToBoid;
    }

    void Alignment(ref Vector2 desired, Boid boid, Vector2 dirToBoid)
    {
        desired += boid.move.vectorVelocity;
    }

    void Cohesion(ref Vector2 desired, Boid boid, Vector2 dirToBoid)
    {
        desired += (Vector2)boid.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!BoidsManager.instance) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(BoidsManager.instance.ViewRadius));
    }
}
