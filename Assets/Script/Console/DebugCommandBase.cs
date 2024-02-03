using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugCommandBase
{
    private string _commandID;
    private string _commandDescription;
    private string _commandFormat;

    public string commandID => _commandID;
    public string commandDescription => _commandDescription;
    public string commandFormat => _commandFormat;

    public DebugCommandBase(string id, string description, string format)
    {
        _commandID = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    } 

    public void Invoke()
    {
        command?.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T value)
    {
        command?.Invoke(value);
    }
}

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
}