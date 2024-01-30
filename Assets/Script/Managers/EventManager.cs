using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//si no se cuantos voy a tener, por q lo desconozco
public class EventManager : SingletonMono<EventManager>
{
    /*
    [SerializeField]
    Pictionarys<string, EventGeneric> _events = new Pictionarys<string, EventGeneric>();

    static public Pictionarys<string, EventGeneric> events => instance._events;
    */
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

/*
public class EventSpecific
{
    System.Delegate listDelegates;

    public void Suscribe(System.Delegate action)
    {
        if (listDelegates == null)
        {
            listDelegates = action;
            return;
        }

        listDelegates = System.Delegate.Combine(listDelegates, action);
    }

    public void Desuscribe(System.Delegate action)
    {
        System.Delegate.Remove(listDelegates, action);
    }

    public virtual void Execute(params object[] param)
    {
        listDelegates.DynamicInvoke(param);
    }

    public static EventSpecific operator -(EventSpecific a, System.Delegate b)
    {
        a.Desuscribe(b);
        return a;
    }

    public static EventSpecific operator +(EventSpecific a, System.Delegate b)
    {
        a.Suscribe(b);
        return a;
    }
}


*/
/*
public class Example
{

    EventSpecific2<System.Action> eventSpecific = new EventSpecific2<System.Action>();

    EventSpecific2<System.Action<int>> eventSpecificInt = new EventSpecific2<System.Action<int>>();

    EventSpecificParent[] eventSpecificParents = new EventSpecificParent[2];

    void Execute()
    {
        eventSpecific += () => { } ;

        eventSpecific.delegato.Invoke();

        eventSpecificInt.delegato.Invoke(5);



        eventSpecificParents[0] = eventSpecific;

        eventSpecificParents[0].delegato.DynamicInvoke("", 5);

        if (eventSpecificParents[0] is EventSpecific2<System.Action>)
        {
            var aux = (EventSpecific2<System.Action>)eventSpecificParents[0];

            aux.delegato.Invoke();
        }
    }

}

public class EventSpecificParent
{
    public System.Delegate delegato { get; protected set; }
}

public class EventSpecific2<T> : EventSpecificParent where T : System.Delegate
{
    new public T delegato { get => (T)base.delegato; private set => base.delegato = value ; }

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

    public static EventSpecific2<T> operator -(EventSpecific2<T> a, T b)
    {
        a.Desuscribe(b);
        return a;
    }

    public static EventSpecific2<T> operator +(EventSpecific2<T> a, T b)
    {
        a.Suscribe(b);
        return a;
    }
}


//Listener se van a llamar handlers

//y los eventos o escuchados events

/*
 
un evento al cual se pueden suscribir, 

 */

/*
public abstract class EventParentSpecific : ScriptableObject
{
    protected System.Delegate listDelegates;

    public void Suscribe(System.Delegate action)
    {
        if (listDelegates == null)
        {
            listDelegates = action;
            return;
        }

        listDelegates = System.Delegate.Combine(listDelegates, action);
    }

    public void Desuscribe(System.Delegate action)
    {
        System.Delegate.Remove(listDelegates, action);
    }


    public static EventParentSpecific operator -(EventParentSpecific a, System.Delegate b)
    {
        a.Desuscribe(b);
        return a;
    }

    public static EventParentSpecific operator +(EventParentSpecific a, System.Delegate b)
    {
        a.Suscribe(b);
        return a;
    }

    public abstract Component CreateMediator(GameObject toAtach);
}


public class EventSpecific<T> : EventParentSpecific where T : System.Delegate
{
    new public T listDelegates { get => (T)base.listDelegates; protected set => base.listDelegates = value; }

    //lo ejecuto en el editor del creator
    public override Component CreateMediator(GameObject toAtach)
    {
        var arrTypes = listDelegates.Method.GetParameters();

        string concat = string.Empty;

        foreach (var item in arrTypes)
        {
            concat += item.ParameterType.Name + ",";
        }

        concat = concat.Remove(concat.Length-1);//tenes q quitar la ultima coma

        return toAtach.AddComponent(System.Type.GetType($"EventMediator<{concat}>"));
    }
}





public abstract class EventMediator : MonoBehaviour
{
    EventSpecific<UnityEngine.Events.UnityAction> suscripted;

    UnityEngine.Events.UnityEvent unityEvent;
}

public abstract class EventMediator<T> : MonoBehaviour
{
    EventSpecific<UnityEngine.Events.UnityAction<T>> suscripted;

    UnityEngine.Events.UnityEvent<T> unityEvent;
}
*/






