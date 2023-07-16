using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//si no se cuantos voy a tener, por q lo desconozco
public class EventManager : SingletonMono<EventManager>
{
    [SerializeField]
    Pictionarys<string, EventGeneric> _events = new Pictionarys<string, EventGeneric>();

    static public Pictionarys<string, EventGeneric> events => instance._events;
}


//si trabajamos con delegados muy genericos
public class EventGeneric
{
    public delegate void _Event(params object[] param);

    public event _Event action;

    public virtual void Execute(params object[] param)
    {
        action?.Invoke(param);
    }
}

public class EventTimer : EventGeneric
{
    public event System.Action end;

    public virtual void ExecuteEnd()
    {
        end?.Invoke();
    }
}

public class EventJoystick : EventTimer
{
    public event _Event set;

    public virtual void ExecuteSet(params object[] param)
    {
        set?.Invoke(param);
    }
}