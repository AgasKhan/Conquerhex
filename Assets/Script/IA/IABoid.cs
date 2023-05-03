using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABoid : IAFather
{
    [SerializeField]
    protected Detect<IGetEntity> detectEnemy;
    [SerializeField]
    protected Detect<IGetEntity> detectItem;


    [SerializeField]
    public Pictionarys<string, SteeringWithTarger> steerings;

    MoveAbstract move;

    delegate void _FuncBoid(ref Vector2 desired, IABoid objective, Vector2 dirToBoid);
    
    public override void OnEnterState(Character param)
    {
        //esto se ejecuta cuando un character me inicia
        move = param.move;

        //necesito añadir a los boid a una lista para usarlos de comparación para los calculos de flocking
        //Manager<IABoid>.pic.Add(GetInstanceID().ToString(), this);
        BoidsManager.list.Add(this);

        //randomizar el movimiento inicial de los boids
        Vector2 random = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        move.Velocity(random.normalized* move.maxSpeed);

    }

    public override void OnExitState(Character param)
    {
        //esto se ejecuta cuando dejo de managear un character
    }

    public override void OnStayState(Character param)
    {

        float distance = float.PositiveInfinity;
        Vector2 dir = Vector2.zero;

        //Debug.Log("enemigo " + steerings["enemigos"].targets.Count + ", recursos " + steerings["frutas"].targets.Count);
        //if (steerings["enemigos"].targets.Count == 0 && steerings["frutas"].targets.Count == 0)

        var enemigo = detectEnemy.Area(param.transform.position, (algo) => { return Team.enemy == algo.GetEntity().team; });
        steerings["enemigos"].targets = enemigo;

        //pendiente: necesito el area para que chequee el mas cercano + chequear que no interfiera con el area de detección del arrive
        var recursos = detectItem.Area(param.transform.position, (target) => { return Team.recursos == target.GetEntity().team; });

        //Si la distancia de mi fruta 1 es menor a la fruta 2, voy a acomodarla para que sea mi primer objetivo
        Entity manzana = null;
        for (int i = 0; i < recursos.Count; i++)
        {
            if (distance > (recursos[i].GetEntity().transform.position - param.transform.position).sqrMagnitude)
            {
                manzana = recursos[i].GetEntity();
                distance = (recursos[i].GetEntity().transform.position - param.transform.position).sqrMagnitude;
            }
        }

        steerings["frutas"].targets.Clear();
        if (manzana != null)
            steerings["frutas"].targets.Add(manzana);


        dir += (Separation() * BoidsManager.instance.SeparationWeight +
                Alignment() * BoidsManager.instance.AlignmentWeight +
               Cohesion() * BoidsManager.instance.CohesionWeight);

        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                dir += itemInPictionary.value[i];
            }
        }


        move.ControllerPressed(dir, 0);
    }

    public void SwitchComportomiento<T>(string key) where T : SteeringBehaviour
    {
        steerings[key].steering = steerings[key].steering.SwitchSteering<T>();
    }

    Vector2 BoidIntern(_FuncBoid func, bool promedio, float radius)
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

    Vector2 Separation()
    {
        return BoidIntern(Separation, false, BoidsManager.instance.SeparationRadius);
    }

    void Separation(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired -= dirToBoid;
    }

    Vector2 Alignment()
    {
        return BoidIntern(Alignment, true, BoidsManager.instance.ViewRadius);
    }

    void Alignment(ref Vector2 desired, IABoid boid, Vector2 dirToBoid)
    {
        desired += boid.move.vectorVelocity;
    }

    Vector2 Cohesion()
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
public class SteeringWithTarger
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
            return steering.Calculate(GetMove(targets[i].GetTransform())) * weight;
        }
    }

    /*
    public Transform GetTransform(IGetEntity getEntity)
    {
        if ((getEntity.GetEntity().transform.position - steering.transform.position).sqrMagnitude > (getEntity.GetTransform().position - steering.transform.position).sqrMagnitude)
            return getEntity.GetTransform();
        else
            return getEntity.GetEntity().transform;
    }

    public Transform GetTransform(int i )
    {
        return GetTransform(targets[i]);
    }
    */

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
