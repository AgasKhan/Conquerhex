using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventController
{
    public event System.Action<Vector2, float> eventDown;
    public event System.Action<Vector2, float> eventPress;
    public event System.Action<Vector2, float> eventUp;
}

public interface IControllerDir
{
    void ControllerDown(Vector2 dir, float tim);
    void ControllerPressed(Vector2 dir, float tim);
    void ControllerUp(Vector2 dir, float tim);
}

public interface IController
{
    void ControllerDown(float tim);
    void ControllerPressed(float tim);
    void ControllerUp(float tim);
}

public interface ICoolDown
{
    public bool onCooldownTime { get; }

    public event System.Action<IGetPercentage, float> onCooldownChange;
}

public class EventControllerMediator : IEventController, IControllerDir
{
    public event Action<Vector2, float> eventDown;
    public event Action<Vector2, float> eventPress;
    public event Action<Vector2, float> eventUp;

    public void ControllerDown(Vector2 dir, float tim)
    {
        eventDown?.Invoke(dir, tim);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        eventPress?.Invoke(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        eventUp?.Invoke(dir, tim);
    }


    public static EventControllerMediator operator - (EventControllerMediator eventController, IControllerDir controllerDir)
    {
        eventController.Remove(controllerDir);
        return eventController;
    }

    public static EventControllerMediator operator + (EventControllerMediator eventController, IControllerDir controllerDir)
    {
        eventController.Add(controllerDir);
        return eventController;
    }

    void Add(IControllerDir controllerDir)
    {
        eventDown += controllerDir.ControllerDown;
        eventPress += controllerDir.ControllerPressed;
        eventUp += controllerDir.ControllerUp;
    }

    void Remove(IControllerDir controllerDir)
    {
        eventDown -= controllerDir.ControllerDown;
        eventPress -= controllerDir.ControllerPressed;
        eventUp -= controllerDir.ControllerUp;
    }
}
