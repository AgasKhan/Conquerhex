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


