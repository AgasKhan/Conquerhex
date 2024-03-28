using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class EventManager : ScriptableObject
{
    [SerializeField]
    Pictionarys<string, Internal.SpecificEventParent> _events = new Pictionarys<string, Internal.SpecificEventParent>();

    public Pictionarys<string, Internal.SpecificEventParent> events => _events;

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

    public void Trigger(string nameOfEvent)
    {
        _events[nameOfEvent].delegato?.DynamicInvoke();
    }

    public void Trigger<T>(string nameOfEvent, T param)
    {
        _events[nameOfEvent].delegato?.DynamicInvoke(param);
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

public class SingleEvent : Internal.SpecificEvent<UnityAction> { };

public class SingleEvent<T1> : Internal.SpecificEvent<UnityAction<T1>> { };

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
