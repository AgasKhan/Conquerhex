using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Patrol
{

    /// <summary>
    /// setea si reinicia la patrulla desde 0, o la recorre una vez terminada en orden inverso
    /// </summary>
    public bool reverse;

    /// <summary>
    /// Indice del array de la patrulla
    /// </summary>
    public int iPatrulla = 0;

    /// <summary>
    /// lista que contiene todos los puntos de la patrulla
    /// </summary>
    public List<Transform> patrol;

    /// <summary>
    /// vector de distancia que falta para llegar al objetivo
    /// </summary>
    [SerializeField]
    Vector3 _distance;

    /// <summary>
    /// Cuanto se desea esperar en cada punto
    /// </summary>
    public
    float _waitTime;

    /// <summary>
    /// referencia privada del monobehabior que le creara
    /// </summary>
    MonoBehaviour _mono;

    public FSMPatrol fsmPatrol;

    /// <summary>
    /// calcula la distancia hasta el numero (pero no lo guarda) punto de patrullaje
    /// </summary>
    /// <returns>retorna el vector de distancia</returns>
    public Vector3 Distance(int i)
    {
        return patrol[i].position - _mono.transform.position;
    }

    /// <summary>
    /// calcula la distancia hasta el siguiente punto de patrullaje
    /// </summary>
    /// <returns>retorna el vector de distancia</returns>
    public Vector3 Distance()
    {
        _distance = patrol[iPatrulla].position - _mono.transform.position;
        return _distance;
    }

    /// <summary>
    /// devuelve el siguiente punto del patrullaje
    /// </summary>
    /// <param name="reverseEffect">devuelve si retrocede o no</param>
    /// <returns>indice del patrullaje</returns>
    public int NextPoint(ref bool reverseEffect)
    {
        int i = iPatrulla;

        if (!reverseEffect)
            i++;
        else
            i--;

        if (i >= patrol.Count)
        {
            if (reverse)
            {
                reverseEffect = !reverseEffect;
                i -= 2;
            }
            else
                i = 0;
        }
        else if (i <= -1)
        {
            i = 0;

            reverseEffect = !reverseEffect;
        }

        return i;
    }

    /// <summary>
    /// chequea si se llego a la distancia minima, y esperara a un timer para ir setear el siguiente punto de patrullaje
    /// </summary>
    /// <param name="minimal"></param>
    /// <returns>En caso de llegar a la distancia minima devolvera un true, por lo contrario un false</returns>
    public bool MinimalChck(float minimal, bool automatic = true)
    {
        if (_distance.sqrMagnitude < minimal * minimal)
        {
            fsmPatrol.CurrentState = fsmPatrol.wait;
            return true;
        }
        return false;
    }

    // Start que configura todo lo necesario para usar la clase
    public void Start(MonoBehaviour m)
    {
        _mono = m;
        fsmPatrol = new FSMPatrol(this);

        if (patrol.Count <= 0)
        {
            GameObject aux = new GameObject(_mono.name + " position");
            aux.transform.position = _mono.transform.position;
            patrol.Add(aux.transform);
        }
    }
}
public interface IPatrolReturn
{
    Patrol PatrolReturn();
}


public class FSMPatrol : FSM<FSMPatrol, Patrol>
{
    public IState<FSMPatrol> move = new Move();

    public IState<FSMPatrol> wait;

    public System.Action OnMove;

    public System.Action OnStartMove;

    public System.Action OnStartWait;

    public System.Action OnWait;

    public FSMPatrol(Patrol reference) : base(reference)
    {
        wait = new Wait(reference._waitTime);
        Init(move);
    }
}

public class Move : IState<FSMPatrol>
{

    public void OnEnterState(FSMPatrol param)
    {
        param.OnStartMove?.Invoke();
    }

    public void OnExitState(FSMPatrol param)
    {
    }

    public void OnStayState(FSMPatrol param)
    {
        param.context.Distance();
        param.OnMove?.Invoke();
    }
}


public class Wait : IState<FSMPatrol>
{
    /// <summary>
    /// Timer que espera una vez alcanzado el destino, para setear el siguiente
    /// </summary>
    public
    Timer timer;

    /// <summary>
    /// privada que se encarga de chequear si tiene q avanzar en la lista o no
    /// </summary>
    bool _reverseEffect;

    public Wait(float _waitTime)
    {
        timer = TimersManager.Create(_waitTime);
    }

    public void OnEnterState(FSMPatrol param)
    {
        timer.Reset();
        param.OnStartWait?.Invoke();
    }

    public void OnExitState(FSMPatrol param)
    {
        param.context.iPatrulla = param.context.NextPoint(ref _reverseEffect);
        param.context.Distance();
    }

    public void OnStayState(FSMPatrol param)
    {
        if (timer.Chck)
        {
            param.CurrentState=param.move;
        }

        param.OnWait?.Invoke();
    }
}