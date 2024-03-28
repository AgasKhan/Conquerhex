using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DebugCommandBase
{
    private string _commandID;
    private string _commandDescription;

    public string commandID => _commandID;
    public string commandDescription => _commandDescription;

    protected EventManager eventManager;

    public DebugCommandBase(string id, string description, ref EventManager eventManager)
    {
        _commandID = id;
        _commandDescription = description;
        this.eventManager = eventManager;
    }
}

public class DebugCommand : DebugCommandBase
{
    public DebugCommand(string id, string description, ref EventManager eventManager) : base(id, description, ref eventManager)
    {

    }

    public void Invoke()
    {
        eventManager.Trigger(commandID);
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    public DebugCommand(string id, string description, ref EventManager eventManager) : base(id, description, ref eventManager)
    {

    }

    public void Invoke(T value)
    {
        eventManager.Trigger<T>(commandID, value);
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