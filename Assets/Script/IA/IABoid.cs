using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABoid : IAFather
{
    [SerializeField]
    protected Detect<Entity> detect;
    [SerializeField]
    protected Detect<IABoid> separa, allignment, cohe;

    [SerializeField]
    public Pictionarys<string,SteeringWithTarger> steerings;

    MoveAbstract move;

    delegate void _FuncBoid(ref Vector2 desired, IABoid objective, Vector2 dirToBoid);

    Vector2 desiredSeparation = Vector2.zero;
    Vector2 desiredAlign = Vector2.zero;
    Vector2 desiredCohesion = Vector2.zero;

    public override void OnEnterState(Character param)
    {
        //esto se ejecuta cuando un character me inicia
        move = param.move;

        //necesito a�adir a los boid a una lista para usarlos de comparaci�n para los calculos de flocking
        //Manager<IABoid>.pic.Add(GetInstanceID().ToString(), this);
        BoidsManager.list.Add(this);

        //randomizar el movimiento inicial de los boids
        Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        move.Velocity(move.maxSpeed).Velocity(random.normalized);

    }

    public override void OnExitState(Character param)
    {
        //esto se ejecuta cuando dejo de managear un character
    }

    public override void OnStayState(Character param)
    {
        Debug.Log("enemigo " + steerings["enemigos"].targets.Count + ", recursos " + steerings["frutas"].targets.Count);
        //if (steerings["enemigos"].targets.Count == 0 && steerings["frutas"].targets.Count == 0)
            //transform.position += transform.right * move.maxSpeed * Time.deltaTime;

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detecci�n del arrive
        var recursos = detect.Area(param.transform.position, (target) => { return Team.recursos == target.team; });

        //a�ado las frutas que estan en mi area de detecci�n, si ya esta en la lista no se a�ade
        steerings["frutas"].targets = recursos;

        //Si la distancia de mi fruta 1 es menor a la fruta 2, voy a acomodarla para que sea mi primer objetivo
        for (int i = 0; i < steerings["frutas"].targets.ToArray().Length-1; i++)
        {
            var distance = (param.transform.position - steerings["frutas"].targets[i].transform.position).magnitude;

            for (int j = 0; j < steerings["frutas"].targets.ToArray().Length; j++)
            {
                var distance2 = (param.transform.position - steerings["frutas"].targets[j].transform.position).magnitude;
                if (distance <= distance2)
                {
                    var aux = steerings["frutas"].targets[i];
                    steerings["frutas"].targets[i] = steerings["frutas"].targets[j];
                    steerings["frutas"].targets[j] = aux;
                }
            }
        }


        var enemigo = detect.Area(param.transform.position, (algo) => { return Team.enemy == algo.team; });

        steerings["enemigos"].targets = enemigo;

        /*
        for (int i = 0; i < recursos.Count; i++)
        {
            if (steerings["frutas"].targets.Contains(recursos[i])) continue;
            steerings["frutas"].targets.Add(recursos[i]);
        }
        */


        ////Intento de autonomia
        //var separation = separa.Area(param.transform.position, (boid) => { return boid.character.team == Team.hervivoro; });
        //foreach (var corderito in separation)
        //{
        //    var dirToCorderito = (corderito.transform.position - param.transform.position).Vect3To2();
        //    desiredSeparation -= dirToCorderito;
        //    Debug.Log("separacion en for " + desiredSeparation);

        //}
        //Debug.Log("separacion " + desiredSeparation);

        //var allign = allignment.Area(param.transform.position, (boid) => { return param.team == boid.character.team; });
        //foreach (var item in allign)
        //{
        //    int count = 0;
        //    var aux = (item.transform.position - param.transform.position).Vect3To2();
        //    desiredAlign += item.move.vectorVelocity;
        //    count++;

        //    if (count > 0)
        //        desiredAlign /= count;
        //}

        //var cohesion = cohe.Area(param.transform.position, (boid) => { return param.team == boid.character.team; });
        //foreach (var item in cohesion)
        //{
        //    int count = 0;
        //    var aux = (item.transform.position - param.transform.position).Vect3To2();

        //    desiredCohesion += (item.transform.position).Vect3To2();
        //    count++;

        //    if (count > 0)
        //        desiredCohesion /= count;

        //    desiredCohesion -= (transform.position).Vect3To2();
        //}

        var flocking = detect.Area(param.transform.position, (algo) => { return param.team == algo.team; });


        move.Acelerator(BoidIntern(Separation, false) * BoidsManager.instance.SeparationWeight +
         BoidIntern(Alignment, true) * BoidsManager.instance.AlignmentWeight +
         BoidIntern(Cohesion, true) * BoidsManager.instance.CohesionWeight);

        // movimiento con intento de autonomia
        //move.Acelerator(desiredSeparation * BoidsManager.instance.SeparationWeight +
        //               desiredAlign * BoidsManager.instance.AlignmentWeight +
        //               desiredCohesion * BoidsManager.instance.CohesionWeight);


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

            //Si esta dentro del rango de vision, seteo un func que variar� seg�n el movimiento que se desea
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
