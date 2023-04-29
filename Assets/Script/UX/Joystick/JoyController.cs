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

        EventManager.events.SearchOrCreate<EventGeneric>(eventController).action += JoyController_action;

        LoadSystem.AddPostLoadCorutine(SetStick);

        //rutina = TimersManager.Create(3, SetStick, false);
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
        var aux = (float)param[0];

        fill = aux > 0.98f ? 1 : aux;
    }
}
