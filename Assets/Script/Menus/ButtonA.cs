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


    public Item myItem;

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

    public ButtonA SetItemName(string name)
    {
        textButton.text = name;
        return this;
    }
    public ButtonA SetItemNum(string num)
    {
        myNum.text = num;
        return this;
    }
    public ButtonA SetItemSprite(Sprite sprite)
    {
        previewImage.sprite = sprite;
        return this;
    }
    public ButtonA SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        listeners += action;
        empty = false;
        return this;
    }
    public ButtonA SetButtonName(string buttonName)
    {
        button.name = buttonName;
        return this;
    }

    public ButtonA SetButtonA(string nameDisplay)
    {
        var item = Manager<ItemBase>.pic[nameDisplay];

        return SetButtonA(item.nameDisplay, item.image, item.nameDisplay, null, item.nameDisplay);
    }
    public ButtonA SetButtonA(string buttonText, Sprite sprite, string otherText, UnityEngine.Events.UnityAction action, string buttonName)
    {
        myItem = Manager<ItemBase>.pic[buttonName].Create();

        textButton.text = buttonText;

        previewImage.sprite = sprite;

        myNum.text = otherText;

        if (buttonName != "")
            button.name = buttonName;
        else
        {
            button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        if (action != null)
        {
            listeners += action;
            empty = false;
        }

        return this;
    }

}
