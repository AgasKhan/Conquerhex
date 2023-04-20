using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyController : MonoBehaviour
{
    [SerializeField]
    KeyInput eventController;

    [SerializeField]
    Stick stick;
    RectTransform rect;

    [SerializeField]
    [Range(0.05f, 1)]
    float deadzone;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;

        stick.AxisButton = VirtualControllers.Search<VirtualControllers.AxisButton>(eventController);
    }
}
