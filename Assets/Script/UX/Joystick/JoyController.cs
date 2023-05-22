using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JoyController : MonoBehaviour
{
    [SerializeField]
    ControllerEnum eventController;

    [SerializeField]
    Stick stick;
    RectTransform rect;

    [SerializeField]
    [Range(0.05f, 1)]
    float deadzone;

    [SerializeField]
    UnityEngine.UI.Image imageToFill;

    //Timer rutina;

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

        axisButton = VirtualControllers.Search<VirtualControllers.AxisButton>(eventController);

        EventManager.events.SearchOrCreate<EventTimer>(eventController).action += JoyController_action;

        EventManager.events.SearchOrCreate<EventTimer>(eventController).end += JoyController_end;

        LoadSystem.AddPostLoadCorutine(SetStick);
    }

    

    void SetStick()
    {
        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;

    }

    private void Update()
    {
        SetStick();
    }

    private void JoyController_action(params object[] param)
    {
        fill = (float)param[0];
    }

    private void JoyController_end()
    {
        fill = 1;
    }
}
