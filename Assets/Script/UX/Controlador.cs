using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlador
{
    public event System.Action<Vector2, float> down;
    public event System.Action<Vector2, float> pressed;
    public event System.Action<Vector2, float> up;
}
