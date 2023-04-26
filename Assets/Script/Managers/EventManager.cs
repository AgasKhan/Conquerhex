using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//si no se cuantos voy a tener, por q lo desconozco
public class EventManager : MonoBehaviour
{
    public static Pictionarys<System.Enum, EventGeneric> events = new Pictionarys<System.Enum, EventGeneric>();

    private void OnDestroy()
    {
        events.Clear();
    }

    private void Awake()
    {
        events.Clear();
    }
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

/*
public enum InterfazController
{
    move,
    principal,
    secondary,
    tertiary,
    life
}*/