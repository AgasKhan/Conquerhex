using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimersManager : MonoBehaviour
{
    static public TimersManager instance;

    [SerializeReference]
    public List<Timer> timersList;

    /// <summary>
    /// Crea un timer que se almacena en una lista para restarlos de forma automatica
    /// </summary>
    /// <param name="totTime2">el tiempo que dura el contador</param>
    /// <param name="m">el multiplicador del contador</param>
    /// <returns>Devuelve la referencia del contador creado</returns>
    public static Timer Create(float totTime2 = 10, float m = 1, bool unscaled=false)
    {
        Timer newTimer = new Timer(totTime2, m, unscaled);
        return newTimer;
    }

    /// <summary>
    /// Crea una rutina que ejecutara una funcion al cabo de un tiempo
    /// </summary>
    /// <param name="totTime">el tiempo total a esperar</param>
    /// <param name="action">la funcion que se ejecutara</param>
    /// <param name="loop">En caso de ser false se quita de la cola, y en caso de ser true se auto reinicia</param>
    /// <returns>retorna la rutina creada</returns>
    public static TimedAction Create(float totTime, Action action)
    {
        TimedAction newTimer = new TimedAction(totTime, action);
        return newTimer;
    }

    /// <summary>
    /// Crea una rutina completa, la cual ejecutara una funcion al comenzar/reiniciar, en el update, y al finalizar
    /// </summary>
    /// <param name="totTime"></param>
    /// <param name="update"></param>
    /// <param name="end"></param>
    /// <param name="loop">En caso de ser false se quita de la cola, y en caso de ser true se auto reinicia</param>
    /// <param name="unscaled"></param>
    /// <returns></returns>
    public static TimedCompleteAction Create(float totTime, Action update, Action end)
    {
        TimedCompleteAction newTimer = new TimedCompleteAction(totTime, update, end);
        return newTimer;
    }

    #region lerps

    static public TimedCompleteAction LerpInTime<T>(T original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(() => original, () => final, seconds, Lerp, save);
    }

    static public TimedCompleteAction LerpInTime<T>(T original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(() => original, final, seconds, Lerp, save);
    }

    static public TimedCompleteAction LerpInTime<T>(System.Func<T> original, T final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        return LerpInTime(original, () => final, seconds, Lerp, save);
    }


    static public TimedCompleteAction LerpInTime<T>(System.Func<T> original, System.Func<T> final, float seconds, System.Func<T, T, float, T> Lerp, System.Action<T> save)
    {
        TimedCompleteAction tim = null;

        System.Action

        update = () =>
        {
            save(Lerp(original(), final(), tim.InversePercentage()));
        }
        ,

        end = () =>
        {
            save(final());
        };

        tim = Create(seconds, update, end);

        return tim;
    }

    static public TimedCompleteAction LerpWithCompare<T>(T original, T final, float velocity, System.Func<T, T, float, T> Lerp, System.Func<T, T, bool> compare, System.Action<T> save)
    {
        TimedCompleteAction tim = null;

        System.Action

        update = () =>
        {
            original = Lerp(original, final, Time.deltaTime * velocity);
            save(original);
            if (compare(original, final))
                tim.Set(0);
            else
                tim.Reset();
        }
        ,
        end = () =>
        {
            save(final);

        };

        tim = Create(1, update, end);

        return tim;
    }

    #endregion




    /// <summary>
    /// Destruye un timer de la lista
    /// </summary>
    /// <param name="timy">El timer que sera destruido</param>
    public static void Destroy(Timer timy)
    {
        instance.timersList.Remove(timy);
    }


    private void Awake()
    {
        timersList = new List<Timer>();
        instance = this;
    }

    void Update()
    {
        for (int i = timersList.Count-1; i >= 0; i--)
        {
            timersList[i].SubsDeltaTime();
        }
    }
}


[System.Serializable]
public class Tim : IGetPercentage
{
    public float total;

    [SerializeField]
    protected float _current;

    public float current
    {
        get => _current;
        set
        {
            _current = value;

            if (_current > total)
                _current = total;
            else if (_current < 0)
                _current = 0;
        }
    }

    /// <summary>
    /// Reinicia el contador a su valor por defecto, para reiniciar la cuenta
    /// </summary>
    public virtual float Reset()
    {
        current = total;

        return total;
    }

    /// <summary>
    /// Efectua una resta en el contador
    /// </summary>
    /// <param name="n">En caso de ser negativo(-) suma al contador, siempre y cuando no este frenado</param>
    public virtual float Substract(float n)
    {
        current -= n;
        return Percentage();
    }

    /// <summary>
    /// Setea el contador
    /// </summary>
    /// <param name="totalTim">El numero a contar</param>
    public Tim Set(float totalTim)
    {
        total = totalTim;
        Reset();

        return this;
    }

    public float Percentage()
    {
        return current / total;
    }

    public float InversePercentage()
    {
        return 1 - Percentage();
    }

    public Tim(float totTim = 10)
    {
        Set(totTim);
    }
}


[System.Serializable]
public class Timer : Tim
{
    float _multiply;
    bool _freeze = true; //por defecto no esta agregado
    protected bool _unscaled;

    protected bool loop;

    public float deltaTime
    {
        get
        {
            return _unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
        }
    }
    
    bool freeze
    {
        get => _freeze;
        set
        {
            if (value == _freeze)
                return;

            if(value)
            {
                TimersManager.instance.timersList.Remove(this);
            }
            else
            {
                TimersManager.instance.timersList.Add(this);
            }

            _freeze = value;
        }
    }

    /// <summary>
    /// Chequea si el contador llego a su fin
    /// </summary>
    /// <returns>Devuelve true si llego a 0</returns>
    public bool Chck
    {
        get
        {
            return _current <= 0;
        }
    }

    /// <summary>
    /// Modifica el numero que multiplica la constante temporal, y asi acelerar o disminuir el timer
    /// </summary>
    /// <param name="m">Por defecto es 1</param>
    public Timer SetMultiply(float m)
    {
        _multiply = m;

        return this;
    }

    /// <summary>
    /// En caso de que el contador este detenido lo reanuda
    /// </summary>
    public Timer Start()
    {
        freeze = false;

        return this;
    }

    /// <summary>
    /// Frena el contador, no resetea ni modifica el contador actual
    /// </summary>
    public Timer Stop(int i = -1)
    {
        if (i < 0)
            freeze = true;
        else
        {
            TimersManager.instance.timersList.RemoveAt(i);
            _freeze = true;
        }

        return this;
    }

    public Timer SetLoop(bool l)
    {
        loop = l;

        return this;
    }

    /// <summary>
    /// Setea el contador, y comienza la cuenta (si se quiere) desde ese numero
    /// </summary>
    /// <param name="totalTim">El numero a contar</param>
    /// <param name="f">Si arranca a contar o no</param>
    public Timer Set(float totalTim, bool f=true)
    {
        base.Set(totalTim);
        freeze = !f;

        return this;
    }

   

    public Timer SetUnscaled(bool u)
    {
        _unscaled = u;

        return this;
    }


    public override float Reset()
    {
        var aux = base.Reset();
        Start();
        return aux;
    }


    /// <summary>
    /// Realiza la resta automatica asi como las funciones necesarias dentro del TimerManager y recibe el indice dentro del manager
    /// </summary>
    /// <returns></returns>
    public virtual float SubsDeltaTime(int i = -1)
    {
        var aux = Substract(deltaTime * _multiply);

        if (aux <= 0)
        {
            if (loop)
                Reset();
            else
                Stop(i);
        }

        return aux;
    }


    /// <summary>
    /// Configura el timer para su uso
    /// </summary>
    /// <param name="totTim">valor por defecto a partir de donde se va a contar</param>
    /// <param name="m">Modifica el multiplicador del timer, por defecto 0</param>
    public Timer(float totTim = 10, float m=1, bool unscaled = false)
    {
        SetMultiply(m);
        SetUnscaled(unscaled);
        Set(totTim);
    }
}

/// <summary>
/// rutina que ejecutara una accion desp de que termine el tiemer
/// </summary>
[System.Serializable] 
public class TimedAction : Timer
{    
    Action end;

    public override float SubsDeltaTime(int i = -1)
    {
        var aux = base.SubsDeltaTime(i);
        if(aux<=0)
        {
            end();
        }

        return aux;
    }

    public TimedAction AddToEnd(Action end)
    {
        this.end += end;

        return this;
    }

    public TimedAction SubstractToEnd(Action end)
    {
        this.end -= end;

        return this;
    }


    public TimedAction(float timer, Action action, bool loop = false, bool unscaled = false) : base(timer)
    {
        this.end = action;
        SetLoop(loop);
        SetUnscaled(unscaled);
    }

}

/// <summary>
/// rutina que ejecutara una funcion al comenzar/reiniciar, otra en cada frame, y otra al final
/// </summary>
[System.Serializable]
public class TimedCompleteAction : TimedAction
{
    Action update;

    /// <summary>
    /// funcion que ejecutara de forma automatica cada frame
    /// </summary>
    public override float SubsDeltaTime(int i = -1)
    {
        update();
        return base.SubsDeltaTime(i);
    }
    

    public TimedCompleteAction AddToUpdate(Action update)
    {
        this.update +=update;

        return this;
    }

    public TimedCompleteAction SubstractToUpdate(Action update)
    {
        this.update -= update;

        return this;
    }



    /// <summary>
    /// crea una rutina que ejecutara una funcion al comenzar/reiniciar, otra en cada frame, y otra al final
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="start"></param>
    /// <param name="update"></param>
    /// <param name="end"></param>
    /// <param name="destroy"></param>
    public TimedCompleteAction(float timer, Action update, Action end, bool loop = false, bool unscaled = false) : base(timer, end, loop, unscaled)
    {
        this.update = update;
    }
}


public interface IGetPercentage
{
    float Percentage();

    float InversePercentage();
}