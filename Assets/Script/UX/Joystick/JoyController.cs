using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyController : MonoBehaviour
{
    [SerializeField]
    EventManager eventsManager;

    [SerializeField]
    bool joystick;

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

    DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)> events;

    //Timer rutina;

    public float fill
    {
        set => imageToFill.fillAmount = value;
        get => imageToFill.fillAmount;
    }

    public Controllers.Axis axisButton
    {
        get => stick.AxisButton;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        LoadSystem.AddPostLoadCorutine(SetStick);

        events = eventsManager.events.SearchOrCreate<DoubleEvent<(IGetPercentage, float), (bool, bool, Sprite)>>(axisButton.name);

        events.secondDelegato += Set;

        events.delegato += JoyController_action;

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

    void Set((bool, bool, Sprite) data)
    {
        gameObject.SetActive(data.Item1 && data.Item2 == joystick);

        imageToReplace.sprite = data.Item3;
    }

    void SetStick()
    {
        stick.initPos = transform.position;

        stick.maxMagnitud = rect.rect.width / 2;

        stick.minMagnitud = stick.maxMagnitud * deadzone;

    }

    private void JoyController_action((IGetPercentage, float) data)
    {
        fill = data.Item1.InversePercentage();
    }


    private void OnEnable()
    {
        fadeOn.FadeOn();
    }
}


