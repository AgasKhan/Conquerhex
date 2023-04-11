using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    public static T instance;

    protected virtual void Awake()
    {
        instance = (T)this;
    }
}

public abstract class SingletonScript<T> : ScriptableObject where T : SingletonScript<T>
{
    static protected T instance;

    protected virtual void Awake()
    {
        instance = (T)this;
    }
}

public abstract class SingletonClass<T> where T : SingletonClass<T>
{
    static protected T instance;

    protected SingletonClass()
    {
        instance = (T)this;
    }
}

public class Manager<T> : SingletonClass<Manager<T>>
{
    List<T> _list = new List<T>();

    static public List<T> list
    {
        get
        {
            if (instance == null)
                new Manager<T>();

            return instance._list;
        }
    }
}


public interface Init
{
    void Init();
}