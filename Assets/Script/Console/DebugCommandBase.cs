using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Internal;

public class DebugCommandBase
{
    protected string _commandID;
    protected string _commandDescription;

    public string commandID => _commandID;
    public string commandDescription => _commandDescription;

    //protected EventManager eventManager;

    public SpecificEventParent specificEvent;

    public DebugCommandBase(string id, string description)
    {
        _commandID = id;
        _commandDescription = description;
        //specificEvent = _specificEvent;
    }
}

public class DebugCommand : DebugCommandBase
{
    public DebugCommand(string id, string description) : base(id, description)
    {

    }

    public void Invoke()
    {
        specificEvent.delegato.DynamicInvoke(commandID);
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    public DebugCommand(string id, string description) : base(id, description)
    {

    }

    public void Invoke(T value)
    {
        specificEvent.delegato.DynamicInvoke(commandID, value);
    }
}
/*
public class DebugCommand<T, V> : DebugCommandBase
{
    private Action<T, V> command;

    public DebugCommand(string id, string description, string format, Action<T, V> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T firstValue, V secondValue)
    {
        command?.Invoke(firstValue, secondValue);
    }
}*/