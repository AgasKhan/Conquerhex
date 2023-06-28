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
    public TextMeshProUGUI myNum;

    public string type;

    public ButtonA SetItemSprite(Sprite sprite)
    {
        previewImage.sprite = sprite;
        return this;
    }

    public ButtonA SetItemName(string name)
    {
        textButton.text = name;
        return this;
    }

    public ButtonA SetType(string type)
    {
        this.type = type;

        return this;

    }

    public ButtonA SetButtonAction(UnityEngine.Events.UnityAction action)
    {
        listeners += action;
        empty = false;
        return this;
    }

    public ButtonA SetItemNum(string num)
    {
        myNum.text = num;
        return this;
    }

    /// <summary>
    /// Crea un ButonA
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="sprite"></param>
    /// <param name="textNum"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public ButtonA SetButtonA(string itemName, Sprite sprite , string textNum, UnityEngine.Events.UnityAction action)
    {
        textButton.text = itemName;

        previewImage.sprite = sprite;

        if (sprite == null)
            previewImage.SetActiveGameObject(false);

        if (textNum != "")
            myNum.text = textNum;
        else
            myNum.SetActiveGameObject(false);

        if (action != null)
        {
            listeners += action;
            empty = false;
        }

        return this;
    }

}
