using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyController : MonoBehaviour
{
    [SerializeField]
    bool joystick;

    [SerializeField]
    EnumController eventController;

    [SerializeField]
    [Range(0.05f, 1)]
    float deadzone;

    [SerializeField]
    FadeOnOff fadeOn;

    [SerializeField]
    Stick stick;

    [SerializeField]
    UnityEngine.UI.Image imageToFill;

    [SerializeField]
    UnityEngine.UI.Image imageToReplace;

    RectTransform rect;

    float imageToFillAlpha;

    float imageToReplaceAlpha;

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

        var events = EventManager.events.SearchOrCreate<EventJoystick>(eventController.ToString());

        events.action += JoyController_action;

        events.set += Set;

        LoadSystem.AddPostLoadCorutine(SetStick);

        imageToFillAlpha = imageToFill.color.a;

        imageToReplaceAlpha = imageToReplace.color.a;

        fadeOn.alphas += FadeOn_alphas;

        fadeOn.Init();
    }

    private void FadeOn_alphas(float obj)
    {
        imageToFill.color = imageToFill.color.ChangeAlphaCopy(obj * imageToFillAlpha);

        imageToReplace.color = imageToReplace.color.ChangeAlphaCopy(obj * imageToReplaceAlpha);
    }

    void Set(params object[] param)
    {
        gameObject.SetActive((bool)param[0] && (bool)param[1] == joystick);

        imageToReplace.sprite = param[2] as Sprite;
    }

    void SetStick()
    {
        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;

    }

    private void JoyController_action(params object[] param)
    {
        fill = ((IGetPercentage)param[0]).InversePercentage();
    }


    private void OnEnable()
    {
        fadeOn.FadeOn();
    }
}


