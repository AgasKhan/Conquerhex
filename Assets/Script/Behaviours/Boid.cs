using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    delegate void _FuncionLoca(ref Vector3 desired, Boid objective, Vector3 dirToBoid);

    [SerializeField] float _maxSpeed;

    [Range(0f, 0.1f), SerializeField]
    float _maxForce;

    Vector3 _velocity;

    void Start()
    {
        Manager<Boid>.pic.Add(GetInstanceID().ToString(), this);

        Vector3 random = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        AddForce(random.normalized * _maxSpeed);
    }

    void Update()
    {
        AddForce(BoidIntern(Separation,false) * BoidsManager.instance.SeparationWeight +
                 BoidIntern(Alignment, true) * BoidsManager.instance.AlignmentWeight +
                 BoidIntern(Cohesion, true) * BoidsManager.instance.CohesionWeight);

        transform.position += _velocity * Time.deltaTime;
       // transform.forward = _velocity;

        CheckBounds();
    }

    void CheckBounds()
    {
        transform.position = GManager.instance.SetObjectBoundPosition(transform.position);
    }

    

    Vector3 BoidIntern(_FuncionLoca func, bool promedio)
    {
        Vector3 desired = Vector3.zero;

        int count = 0;

        //Por cada boid
        foreach (var boid in Manager<Boid>.pic)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid.value == this) continue;

            //Saco la direccion hacia el boid
            Vector3 dirToBoid = boid.value.transform.position - transform.position;

            //Si esta dentro del rango de vision
            if (dirToBoid.sqrMagnitude <= BoidsManager.instance.SeparationRadius)
            {
                //En este caso me resto porque quiero separarme hacia el lado contrario
                func(ref desired, boid.value, dirToBoid);

                count++;
            }
        }

        if (desired == Vector3.zero) return desired;

        if(promedio)
            desired /= count;

        return CalculateSteering(desired);
    }

    void Separation(ref Vector3 desired, Boid boid, Vector3 dirToBoid)
    {
        desired -= dirToBoid;
    }

    void Alignment(ref Vector3 desired, Boid boid, Vector3 dirToBoid)
    {
        desired += boid._velocity;
    }

    void Cohesion(ref Vector3 desired, Boid boid, Vector3 dirToBoid)
    {
        desired += boid.transform.position;
    }

    Vector3 CalculateSteering(Vector3 desired)
    {
        //desired.Normalize();
        //desired *= _maxSpeed;
        //desired -= _velocity;

        //return Vector3.ClampMagnitude(desired, _maxForce);

        return Vector3.ClampMagnitude((desired.normalized * _maxSpeed) - _velocity, _maxForce);
    }

    void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    private void OnDrawGizmos()
    {
        if (!BoidsManager.instance) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(BoidsManager.instance.ViewRadius));
    }
}
