using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABoid : IAFather
{
    [SerializeField]
    protected Detect<IGetEntity> detectEnemy;
    [SerializeField]
    protected Detect<IGetEntity> detectObjective;


    [SerializeField]
    public Pictionarys<string, SteeringWithTarget> steerings;

    MoveAbstract move;

    protected Vector2 dir = Vector2.zero;

    delegate void _FuncBoid(ref Vector2 desired, IABoid objective, Vector2 dirToBoid);
    
    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        move = param.move.move;

        BoidsManager.list.Add(this);

        //randomizar el movimiento inicial de los boids
        Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        move.ControllerPressed(random.normalized,0);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        Detection();


        Flocking();


        SteeringsMovement();


        move.ControllerPressed(dir, 0);
    }

    protected virtual void Detection()
    {
        float distance = float.PositiveInfinity;

        dir = Vector2.zero;

        var enemigo = detectEnemy.Area(_character.transform.position, (algo) => { return _character.team != algo.GetEntity().team && Team.recursos != algo.GetEntity().team; });
        steerings["enemigos"].targets = enemigo;

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detección del arrive
        var recursos = detectObjective.Area(_character.transform.position, (target) => { return Team.recursos == target.GetEntity().team; });

        //Si la distancia de mi fruta 1 es menor a la fruta 2, voy a acomodarla para que sea mi primer objetivo
        Entity manzana = null;
        for (int i = 0; i < recursos.Count; i++)
        {
            if (distance > (recursos[i].GetEntity().transform.position - _character.transform.position).sqrMagnitude)
            {
                manzana = recursos[i].GetEntity();
                distance = (recursos[i].GetEntity().transform.position - _character.transform.position).sqrMagnitude;
            }
        }

        steerings["frutas"].targets.Clear();
        if (manzana != null)
            steerings["frutas"].targets.Add(manzana);
    }

    protected void SteeringsMovement()
    {
        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                dir += itemInPictionary.value[i];
            }
        }
    }

    protected virtual void Flocking()
    {
        dir += (Separation() * BoidsManager.instance.SeparationWeight +
                Alignment() * BoidsManager.instance.AlignmentWeight +
               Cohesion() * BoidsManager.instance.CohesionWeight);
    }


    Vector2 BoidIntern(_FuncBoid func, bool promedio, float radius)
    {
        Vector2 desired = Vector2.zero;
        int count = 0;

        //Por cada boid
        foreach (var boid in BoidsManager.list)
        {
            //Si soy este boid o es mi enemigo, ignoro y sigo la iteracion
            if (boid == this || boid._character.team != _character.team) continue;

            //Saco la direccion hacia el boid
            Vector2 dirToBoid = boid.transform.position - transform.position; //seek.Calculate(boid.value.move);

            //Si esta dentro del rango de vision, seteo un func que variará según el movimiento que se desea

            if (dirToBoid.sqrMagnitude <= radius)
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

    protected Vector2 Separation()
    {
        return BoidIntern(Separation, false, BoidsManager.instance.SeparationRadius);
    }

    void Separation(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired -= dirToBoid;
    }

    protected Vector2 Alignment()
    {
        return BoidIntern(Alignment, true, BoidsManager.instance.ViewRadius);
    }

    void Alignment(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired += boid.move.vectorVelocity;
    }

    protected Vector2 Cohesion()
    {
        var aux = BoidIntern(Cohesion, true, BoidsManager.instance.ViewRadius);

        return aux == Vector2.zero ? Vector2.zero : aux - transform.position.Vect3To2();
    }

    void Cohesion(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired += (Vector2)boid.transform.position;
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
public class SteeringWithTarget
{
    public SteeringBehaviour steering;
    public float weight = 1;

    public List<IGetEntity> targets = new List<IGetEntity>();

    public void SwitchSteering<T>() where T : SteeringBehaviour
    {
        steering = steering.SwitchSteering<T>();
    }

    public int Count => targets.Count;

    Dictionary<Transform, MoveAbstract> lookUpTable = new Dictionary<Transform, MoveAbstract>();

    //version hiper justificada de un lookuptable

    /// <summary>
    /// LookUpTable de obtencion de MoveAbstracts
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Vector2 this[int i]
    {
        get
        {
            return steering.Calculate(GetMove(targets[i].transform)) * weight;
        }
    }

    public MoveAbstract GetMove(Transform tr)
    {
        MoveAbstract aux;

        //en caso que mi diccionario sea muy largo es mas rapido directamente obtener el componente
        if (lookUpTable.Count <= 200)
        {
            if (lookUpTable.TryGetValue(tr, out aux))
            {
                return aux;
            }
        }

        if (!tr.TryGetComponent(out aux))
        {
            aux = tr.gameObject.AddComponent<MoveTr>();
            aux.enabled = false;
        }

        lookUpTable.Add(tr.transform, aux);

        return aux;
    }
}
