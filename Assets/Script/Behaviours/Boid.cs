using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    Vector3 _velocity;

    [SerializeField] float _maxSpeed;

    [Range(0f, 0.1f), SerializeField]
    float _maxForce;

    void Start()
    {
        BoidsManager.Instance.RegisterNewBoid(this);

        Vector3 random = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        AddForce(random.normalized * _maxSpeed);
    }

    void Update()
    {
        AddForce(Separation() * BoidsManager.Instance.SeparationWeight +
                 Alignment() * BoidsManager.Instance.AlignmentWeight +
                 Cohesion() * BoidsManager.Instance.CohesionWeight);

        transform.position += _velocity * Time.deltaTime;
       // transform.forward = _velocity;

        CheckBounds();
    }

    void CheckBounds()
    {
        transform.position = GManager.Instance.SetObjectBoundPosition(transform.position);
    }

    Vector3 Separation()
    {
        //Variable donde vamos a recolectar todas las direcciones entre los flockmates
        Vector3 desired = Vector3.zero;

        //Por cada boid
        foreach (Boid boid in BoidsManager.Instance.AllBoids)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid == this) continue;

            //Saco la direccion hacia el boid
            Vector3 dirToBoid = boid.transform.position - transform.position;

            //Si esta dentro del rango de vision
            if (dirToBoid.sqrMagnitude <= BoidsManager.Instance.SeparationRadius)
            {
                //En este caso me resto porque quiero separarme hacia el lado contrario
                desired -= dirToBoid;
            }
        }

        if (desired == Vector3.zero) return desired;

        return CalculateSteering(desired);
    }

    Vector3 Alignment()
    {
        //Variable donde vamos a recolectar todas las direcciones entre los flockmates
        Vector3 desired = Vector3.zero;

        //Contador para acumular cantidad de boids a promediar
        int count = 0;

        //Por cada boid
        foreach (Boid boid in BoidsManager.Instance.AllBoids)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid == this) continue;

            //Saco la direccion hacia el boid
            Vector3 dirToBoid = boid.transform.position - transform.position;

            //Si esta dentro del rango de vision
            if (dirToBoid.sqrMagnitude <= BoidsManager.Instance.ViewRadius)
            {
                //Sumo la direccion hacia donde esta yendo el boid
                desired += boid._velocity;

                //Sumo uno mas a mi contador para promediar
                count++;
            }
        }

        if (count == 0) return desired;

        //Promediamos todas las direcciones
        desired /= count;

        return CalculateSteering(desired);
    }

    Vector3 Cohesion()
    {
        //Variable donde vamos a recolectar todas las direcciones entre los flockmates
        Vector3 desired = Vector3.zero;

        //Contador para acumular cantidad de boids a promediar
        int count = 0;

        foreach (Boid boid in BoidsManager.Instance.AllBoids)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid == this) continue;

            //Saco la direccion hacia el boid
            Vector3 dirToBoid = boid.transform.position - transform.position;

            //Si esta dentro del rango de vision
            if (dirToBoid.sqrMagnitude <= BoidsManager.Instance.ViewRadius)
            {
                //Sumo la posicion de cada boid
                desired += boid.transform.position;

                //Sumo uno mas a mi contador para promediar
                count++;
            }
        }

        if (count == 0) return desired;

        //Promediamos todas las posiciones
        desired /= count;

        //Restamos nuestra posicion para que tambien sea parte de la cohesion
        desired -= transform.position;

        return CalculateSteering(desired);
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
        if (!BoidsManager.Instance) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(BoidsManager.Instance.ViewRadius));
    }
}
