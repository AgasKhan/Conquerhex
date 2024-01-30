using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Managers/EventManager")]
public class NewEventManager : ScriptableObject
{
    [SerializeField]
    Pictionarys<string, Euler.SpecificEventParent> _events = new Pictionarys<string, Euler.SpecificEventParent>();

    public Pictionarys<string, Euler.SpecificEventParent> events => _events;

    public Euler.SpecificEventParent this[string k]
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

    public void MyOnDestroy()
    {
        for (int i = 0; i < _events.Count; i++)
        {
            if (_events[i] != null)
                _events[i].delegato = null;
        }
    }
}

public class EventParam : Euler.SpecificEvent<UnityAction> { };

public class EventParam<T1> : Euler.SpecificEvent<UnityAction<T1>> { };

public class EventParam<T1, T2> : Euler.SpecificEvent<UnityAction<T1, T2>> { };

public class EventParam<T1, T2, T3> : Euler.SpecificEvent<UnityAction<T1, T2, T3>> { };


public class EventTwoParam : Euler.SpecificEvent<UnityAction>
{
    public UnityAction secondDelegato { get => _second.delegato; set => _second.delegato = value; }

    EventParam _second = new EventParam();
}
public class EventTwoParam<X1, Y1> : Euler.SpecificEvent<UnityAction<X1>>
{
    public UnityAction<Y1> secondDelegato { get => _second.delegato; set => _second.delegato = value; }

    EventParam<Y1> _second = new EventParam<Y1>();
}
public class EventTwoParam<X1, X2, Y1, Y2> : Euler.SpecificEvent<UnityAction<X1, X2>>
{
    public UnityAction<Y1, Y2> secondDelegato { get => _second.delegato; set => _second.delegato = value; }

    EventParam<Y1, Y2> _second = new EventParam<Y1, Y2>();
}

public class EventThreeParam : EventTwoParam
{
    public UnityAction thirdDelegato { get => _third.delegato; set => _third.delegato = value; }

    EventParam _third = new EventParam();
}
public class EventThreeParam<X1, Y1, Z1> : EventTwoParam<X1, Y1>
{
    public UnityAction<Z1> thirdDelegato { get => _third.delegato; set => _third.delegato = value; }

    EventParam<Z1> _third = new EventParam<Z1>();
}


namespace Euler
{
    public class SpecificEventParent
    {
        public System.Delegate delegato { get; set; }
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