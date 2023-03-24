using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyController : MonoBehaviour, IControlador
{
    [SerializeField]
    Stick stick;
    RectTransform rect;

    [SerializeField]
    [Range(0.05f, 1)]
    float deadzone;

    public event Action<Vector2, float> down
    {
        add
        {
            stick.down += value;
        }
        remove
        {
            stick.down -= value;
        }
    }
    public event Action<Vector2, float> pressed
    {
        add
        {
            stick.pressed += value;
        }
        remove
        {
            stick.pressed -= value;
        }
    }
    public event Action<Vector2, float> up
    {
        add
        {
            stick.up += value;
        }
        remove
        {
            stick.up -= value;
        }
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;
    }

   


}
