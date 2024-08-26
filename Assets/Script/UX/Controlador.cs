using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventController
{
    public event System.Action<Vector2, float> eventDown;
    public event System.Action<Vector2, float> eventPress;
    public event System.Action<Vector2, float> eventUp;

    public void SuscribeController(IControllerDir controllerDir);

    public void DesuscribeController(IControllerDir controllerDir);
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

    public Func<Quaternion> quaternion
    {
        set
        {
            if(value == null)
            {
                _quaternion = QuaterionIdentity;
            }
            else
            {
                _quaternion = value;
            }
        }
    }

    Func<Quaternion> _quaternion = QuaterionIdentity;

    //Quaternion _quaternion => quaternion == null ? Quaternion.identity : quaternion();

    static Quaternion QuaterionIdentity() => Quaternion.identity;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
        }
    }

    bool _enabled = true;

    public void ControllerDown(Vector2 dir, float tim)
    {
        if(Enabled)
            eventDown?.Invoke(_quaternion() * dir, tim);
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (Enabled)
            eventPress?.Invoke(_quaternion() * dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (Enabled)
            eventUp?.Invoke(_quaternion() * dir, tim);
    }


    public static EventControllerMediator operator - (EventControllerMediator eventController, IControllerDir controllerDir)
    {
        eventController.DesuscribeController(controllerDir);
        return eventController;
    }

    public static EventControllerMediator operator + (EventControllerMediator eventController, IControllerDir controllerDir)
    {
        eventController.SuscribeController(controllerDir);
        return eventController;
    }

    public void SuscribeController(IControllerDir controllerDir)
    {
        eventDown += controllerDir.ControllerDown;
        eventPress += controllerDir.ControllerPressed;
        eventUp += controllerDir.ControllerUp;
    }

    public void DesuscribeController(IControllerDir controllerDir)
    {
        eventDown -= controllerDir.ControllerDown;
        eventPress -= controllerDir.ControllerPressed;
        eventUp -= controllerDir.ControllerUp;
    }
}
