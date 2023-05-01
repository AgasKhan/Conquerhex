using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Patrol : Init
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
    public Transform patrolParent;

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
        return patrolParent.GetChild(i).position - _mono.transform.position;
    }

    /// <summary>
    /// calcula la distancia hasta el siguiente punto de patrullaje
    /// </summary>
    /// <returns>retorna el vector de distancia</returns>
    public Vector3 Distance()
    {
        _distance = patrolParent.GetChild(iPatrulla).position - _mono.transform.position;
        return _distance;
    }

    /// <summary>
    /// devuelve el siguiente punto del patrullaje
    /// </summary>
    /// <param name="reverseEffect">devuelve si retrocede o no</param>
    /// <returns>indice del patrullaje</returns>
    /// 

    int reverseEffect = 1;

    int NextPointCircle()
    {
        int i = iPatrulla;

        i += reverseEffect;

        if (i <= -1 || i >= patrolParent.childCount)
        {
            i = reverseEffect == 1 ? i-2 : 1;
            reverseEffect *= -1;
        }

        return i;
    }

    int NextPointLineal()
    {
        int i = iPatrulla;

        i += reverseEffect;

        if (i <= -1 || i >= patrolParent.childCount)
        {
            i = reverseEffect== 1 ? 0 : patrolParent.childCount;
            reverseEffect *= -1;
        }

        return i;
    }

    public int NextPoint()
    {
        return reverse ? NextPointCircle() : NextPointLineal();
    }


    /*Version antigua
    public int NextPoint(ref bool reverseEffect)
    {
        int i = iPatrulla;

        if (!reverseEffect)
            i++;
        else
            i--;

        if (i >= patrolParent.childCount)
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
    }*/

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

    /// <summary>
    /// Init que se debera ejecutar para el correcto funcionamiento de la patrulla
    /// </summary>
    /// <param name="param">El primer parametro debe de ser el MonoBehaviour que lo crea</param>
    public void Init(params object[] param)
    {
        _mono = param[0] as MonoBehaviour;
        fsmPatrol = new FSMPatrol(this);

        if (patrolParent.childCount <= 0)
        {
            GameObject aux = new GameObject(_mono.name + " position");
            aux.transform.position = _mono.transform.position;
            aux.transform.parent = patrolParent;
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

    public System.Action OnStartMove;

    public System.Action OnMove;

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
    public Timer timer;

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
        param.context.iPatrulla = param.context.NextPoint();
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