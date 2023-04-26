using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

    [SerializeField]
    UnityEngine.UI.Image imageToFill;

    public float fill
    {
        set => imageToFill.fillAmount = value;
        get => imageToFill.fillAmount;
    }

    public VirtualControllers.AxisButton axisButton
    {
        get => stick.AxisButton;
        private set => stick.AxisButton = value;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;

        axisButton = VirtualControllers.Search<VirtualControllers.AxisButton>(eventController);
    }
}
