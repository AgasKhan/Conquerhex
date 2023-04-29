using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IABoid : IAFather
{
    [SerializeField]
    protected Detect<Entity> detect;
    //protected Detect<IABoid> a, b, c;

    [SerializeField]
    public Pictionarys<string,SteeringWithTarger> steerings;

    MoveAbstract move;

    delegate void _FuncBoid(ref Vector2 desired, IABoid objective, Vector2 dirToBoid);
    List<IABoid> list = new List<IABoid>();
    public override void OnEnterState(Character param)
    {
        //esto se ejecuta cuando un character me inicia
        move = param.move;

        //necesito añadir a los boid a una lista para usarlos de comparación para los calculos de flocking
        //Manager<IABoid>.pic.Add(GetInstanceID().ToString(), this);
        BoidsManager.list.Add(this);

        //randomizar el movimiento de los boids
        Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        move.Velocity(move.maxSpeed).Velocity(random.normalized);

    }

    public override void OnExitState(Character param)
    {
        //esto se ejecuta cuando dejo de managear un character
    }

    public override void OnStayState(Character param)
    {

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detección del arrive
        var recursos = detect.Area(param.transform.position, (target) => { return Team.recursos == target.team; });

        //añado las frutas que estan en mi area de detección, si ya esta en la lista no se añade
        steerings["frutas"].targets = recursos;

        var enemigo = detect.Area(param.transform.position, (algo) => { return Team.enemy == algo.team; });

        steerings["enemigos"].targets = enemigo;

        /*
        for (int i = 0; i < recursos.Count; i++)
        {
            if (steerings["frutas"].targets.Contains(recursos[i])) continue;
            steerings["frutas"].targets.Add(recursos[i]);
        }
        */




        //var separation = a.Area(param.transform.position, (algo) => { return param.team == algo.team; });
        //var flocking = detect.Area(param.transform.position, (algo) => { return param.team == algo.team; });


        move.Acelerator(BoidIntern(Separation, false) * BoidsManager.instance.SeparationWeight +
         BoidIntern(Alignment, true) * BoidsManager.instance.AlignmentWeight +
         BoidIntern(Cohesion, true) * BoidsManager.instance.CohesionWeight);


        /*
        for (int i = 0; i < enemigo.Length; i++)
        {
            
            if(!steerings["enemigos"].targets.Contains(enemigo[i]) && (enemigo[i].transform.position - transform.position).sqrMagnitude <= detect.radius * detect.radius)
            {
                steerings["enemigos"].targets.Add(enemigo[i]);
            }
            else if((enemigo[i].transform.position - transform.position).sqrMagnitude > detect.radius * detect.radius)
                steerings["enemigos"].targets.Remove(enemigo[i]);
          
        }
        */

        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                move.Acelerator(itemInPictionary.value.steering.Calculate(itemInPictionary.value[i]));
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
        foreach (var boid in BoidsManager.list)
        {
            //Si soy este boid a chequear, ignoro y sigo la iteracion
            if (boid == this) continue;

            //Saco la direccion hacia el boid
            Vector2 dirToBoid = boid.transform.position - transform.position; //seek.Calculate(boid.value.move);

            //Si esta dentro del rango de vision, seteo un func que variará según el movimiento que se desea
            if (dirToBoid.sqrMagnitude <= BoidsManager.instance.SeparationRadius)
            {
                func(ref desired, boid, dirToBoid);

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

        //CheckBounds();
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
public class SteeringWithTarger
{
    public SteeringBehaviour steering;

    public List<Entity> targets = new List<Entity>();

    public int Count => targets.Count;

    Dictionary<Entity, MoveAbstract> lookUpTable = new Dictionary<Entity, MoveAbstract>();

    //version hiper justificada de un lookuptable

    /// <summary>
    /// LookUpTable de obtencion de MoveAbstracts
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
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
