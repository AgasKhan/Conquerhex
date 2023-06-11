using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonA : EventsCall
{
    [SerializeField]
    Image previewImage;

    [SerializeField]
    TextMeshProUGUI myNum;

    public ButtonA SetButtonA(Sprite sprite, EventsCall buttonEventsCall, string text)
    {
        previewImage.sprite = sprite;

        // Configuracion del botón 
        button = buttonEventsCall.button;
        textButton = buttonEventsCall.textButton;
        fadeMenu = buttonEventsCall.fadeMenu;

        textButton.text = text;

        return this;
    }
}
