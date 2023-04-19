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
    Pictionarys<string,T> _pic = new Pictionarys<string,T>();

    static public Pictionarys<string,T> pic
    {
        get
        {
            if (instance == null)
                new Manager<T>();

            return instance._pic;
        }
    }
}


public class ManagerAddRemove<T>
{
    //referencia de mi pic que si es estatico
    Pictionarys<string, T> _pic = Manager<T>.pic;

    List<string> keys = new List<string>();

    public void RemoveAll()
    {
        foreach (var item in keys)
        {
            _pic.Remove(item);
        }

        keys.Clear();
    }

    public void Add(string key, T value)
    {
        keys.Add(key);

        _pic.Add(key, value);
    }

    ~ManagerAddRemove()
    {
        RemoveAll();
    }
}

/*
public class ManagerComponent<T> : SingletonMono<ManagerComponent<T>>
{
    [SerializeReference]
    Pictionarys<string, T> _pic = new Pictionarys<string, T>();

    static public Pictionarys<string, T> pic
    {
        get
        {
            if (instance == null)
                GameManager.instance.gameObject.AddComponent<ManagerComponent<T>>();

            return instance._pic;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}

*/



public interface Init
{
    void Init();
}