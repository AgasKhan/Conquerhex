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
    public static T instance;

    protected virtual void Awake()
    {
        instance = (T)this;
    }
}

public abstract class Manager<T> : SingletonScript<T> where T : Manager<T>
{
    protected void InitAll(IEnumerable<Init> init)
    {
        foreach (var item in init)
        {
            item.Init();
        }
    }
}


public interface Init
{
    void Init();
}