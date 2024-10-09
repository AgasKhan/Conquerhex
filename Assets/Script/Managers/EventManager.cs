using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class EventManager : ScriptableObject
{
    [SerializeField]
    Pictionarys<string, Internal.SpecificEventParent> _events = new Pictionarys<string, Internal.SpecificEventParent>();

    public Pictionarys<string, Internal.SpecificEventParent> events => _events;

    [SerializeField]
    string command;

    public Internal.SpecificEventParent this[string k]
    {
        get
        {
            return _events[k];
        }

        set
        {
            _events[k] = value;
        }
    }

    [ContextMenu("Command")]
    void Trigger()
    {
        TriggerCommand(command);
    }

    public void Trigger(string nameOfEvent)
    {
        _events[nameOfEvent].delegato?.DynamicInvoke();
    }

    public void Trigger<T>(string nameOfEvent, T param)
    {
        _events[nameOfEvent].delegato?.DynamicInvoke(param);
    }

    public void TriggerCommand(string command)
    {
        var parameters = command.Split(" ");

        //var paramtersType = ;
        bool succes = true;

        List<object> parametersConverted = new List<object>();

        foreach (var paramtersType in _events[parameters[0]].GetType().GetGenericArguments())
        {
            Debug.Log($"{paramtersType.FullName}");
            if (paramtersType == typeof(string))
            {
                parametersConverted.Add(parameters[parametersConverted.Count + 1]);
            }
            else if (paramtersType == typeof(int))
            {
                parametersConverted.Add(int.Parse(parameters[parametersConverted.Count + 1]));
            }
            else if (paramtersType == typeof(float))
            {
                parametersConverted.Add(float.Parse(parameters[parametersConverted.Count + 1]));
            }
            else
            {
                succes = false;
                Debug.LogError("parameter not converted");
                break;
            }
        }

        if(succes)
            _events[parameters[0]].delegato?.DynamicInvoke(parametersConverted.ToArray());
    }


    public void MyOnDestroy()
    {
        /*
        for (int i = 0; i < _events.Count; i++)
        {
            if (_events[i] != null)
                _events[i].Clear();
        }
        */

        _events.Clear();
    }
}

/*
public class DelegateAutoDesuscription<T> where T : Delegate
{
    HashSet<T> listOfDelegates = new HashSet<T>();

    public event T @event
    {
        add
        {
            listOfDelegates.Add(value);
        }

        remove
        {
            listOfDelegates.Remove(value);
        }
    }
}
*/


public class SingleEvent : Internal.SpecificEvent<UnityAction> { };

public class SingleEvent<T1> : Internal.SpecificEvent<UnityAction<T1>> { };

public class SingleEvent<T1, T2> : Internal.SpecificEvent<UnityAction<T1, T2>> { };

public class SingleEvent<T1, T2, T3> : Internal.SpecificEvent<UnityAction<T1, T2, T3>> { };

public class DoubleEvent : SingleEvent
{
    public UnityAction secondDelegato;

    public override void Clear()
    {
        base.Clear();
        secondDelegato = null;
    }
}

public class DoubleEvent<T1, T2> : Internal.SpecificEvent<UnityAction<T1>>
{
    public UnityAction<T2> secondDelegato;

    public override void Clear()
    {
        base.Clear();
        secondDelegato = null;
    }
}

public class TripleEvent : DoubleEvent
{
    public UnityAction thirdDelegato;

    public override void Clear()
    {
        base.Clear();
        thirdDelegato = null;
    }
}

public class TripleEvent<T1, T2, T3> : DoubleEvent<T1, T2>
{
    public UnityAction<T3> thirdDelegato;

    public override void Clear()
    {
        base.Clear();
        thirdDelegato = null;
    }
}



namespace Internal
{
    public class SpecificEventParent
    {
        public System.Delegate delegato { get; set; }

        public virtual void Clear()
        {
            delegato = null;
        }
    }

    public class SpecificEvent<T> : SpecificEventParent where T : System.Delegate
    {
        public new T delegato { get => (T)base.delegato; set => base.delegato = value; }

        public void Suscribe(T action)
        {
            if (delegato == null)
            {
                delegato = action;
                return;
            }

            delegato = (T)System.Delegate.Combine(delegato, action);
        }

        public void Desuscribe(T action)
        {
            delegato = (T)System.Delegate.Remove(delegato, action);
        }

        public static SpecificEvent<T> operator -(SpecificEvent<T> a, T b)
        {
            a.Desuscribe(b);
            return a;
        }

        public static SpecificEvent<T> operator +(SpecificEvent<T> a, T b)
        {
            a.Suscribe(b);
            return a;
        }
    }
}
