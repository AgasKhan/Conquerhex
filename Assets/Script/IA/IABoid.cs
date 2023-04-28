using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IABoid : IAFather
{
    [SerializeField]
    Detect<Entity> detect;

    [SerializeField]
    Pictionarys<string,SteeringWithTarger> steerings;

    MoveAbstract move;

    delegate void _FuncBoid(ref Vector2 desired, IABoid objective, Vector2 dirToBoid);

    public override void OnEnterState(Character param)
    {
        //esto se ejecuta cuando un character me inicia

        //necesito añadir a los boid a una lista para usarlos de comparación para los calculos de flocking
    //    Manager<IABoid>.pic.Add(GetInstanceID().ToString(), this);

        //randomizar el movimiento de los boids
    //    Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    //    param.move.Velocity(param.move.maxSpeed).Velocity(random.normalized);
    //
    }

    public override void OnExitState(Character param)
    {
        //esto se ejecuta cuando dejo de managear un character
    }

    public override void OnStayState(Character param)
    {
       
        //var flocking = detect.Area(param.transform.position, (algo) => { return param.team == algo.team; });
        //Execute();

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detección del arrive
        var recursos = detect.Area(param.transform.position, (target) => { return Team.recursos == target.team;});

        //quiero recorrer la cantidad de gameobjects que tienen el team recursos para añadirlos a la lista correspondiente, en este caso, fruta
        for (int i = 0; i < recursos.Length; i++)
        {
            steerings["frutas"].targets.Add(recursos[i].transform);
            Debug.Log("recursos en for " + recursos[i]);
        }

        //var enemigo = detect.Area(param.transform.position, (algo) => { return Team.enemy == algo.team; });


        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                param.move.Acelerator(itemInPictionary.value.steering.Calculate(itemInPictionary.value[i]));
            }
        }
    }

    public void SwitchComportomiento<T>(string key) where T : SteeringBehaviour
    {
        steerings[key].steering = steerings[key].steering.SwitchSteering<T>();
    }

    Vector2 BoidIntern(_FuncBoid func, bool promedio)
    {
        Vector2 desired = Vector2.zero;

        int count = 0;

        //Por cada boid
        foreach (var boid in Manager<IABoid>.pic)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid.value == this) continue;

            //Saco la direccion hacia el boid
            Vector2 dirToBoid = boid.value.transform.position - transform.position; //seek.Calculate(boid.value.move);

            //Si esta dentro del rango de vision, seteo un func que variará según el movimiento que se desea
            if (dirToBoid.sqrMagnitude <= BoidsManager.instance.SeparationRadius)
            {
                func(ref desired, boid.value, dirToBoid);

                count++;
            }
        }

        if (desired == Vector2.zero) return desired;

        //En caso de requerir tener el promedio de todos los boids, promedio con mi desired
        if (promedio)
            desired /= count;

        return desired;
    }

    void Separation(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired -= dirToBoid;
    }

    void Alignment(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired += boid.move.vectorVelocity;
    }

    void Cohesion(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired += (Vector2)boid.transform.position;
    }

    //Veo los limites
    void CheckBounds()
    {
        transform.position = Boundaries.instance.SetObjectBoundPosition(transform.position);
    }

    //Ejecuto el mov flocking (align, separation, cohesion)
    void Execute()
    {
        move.Acelerator(BoidIntern(Separation, false) * BoidsManager.instance.SeparationWeight +
         BoidIntern(Alignment, true) * BoidsManager.instance.AlignmentWeight +
         BoidIntern(Cohesion, true) * BoidsManager.instance.CohesionWeight);

        CheckBounds();
    }

    //Ver gizmos
    private void OnDrawGizmos()
    {
        if (!BoidsManager.instance) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(BoidsManager.instance.ViewRadius));
    }
}


[System.Serializable]
class SteeringWithTarger
{
    public SteeringBehaviour steering;

    public List<Transform> targets = new List<Transform>();

    public int Count => targets.Count;

    Dictionary<Transform, MoveAbstract> lookUpTable = new Dictionary<Transform, MoveAbstract>();

    //version hiper justificada de un lookuptable
    public MoveAbstract this[int i]
    {
        get
        {
            MoveAbstract aux;

            //en caso que mi diccionario sea muy largo es mas rapido directamente obtener el componente
            if (lookUpTable.Count<=200)
            {
                if (lookUpTable.TryGetValue(targets[i], out aux))
                {
                    return aux;
                }
            }

            if (!targets[i].TryGetComponent(out aux))
            {
                aux = targets[i].gameObject.AddComponent<MoveTr>();
                aux.enabled = false;
            }

            lookUpTable.Add(targets[i], aux);
  

            return aux;

        }
    }


}
