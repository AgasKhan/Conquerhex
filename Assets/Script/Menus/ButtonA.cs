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


    public ButtonA SetButtonA(string nameDisplay)
    {
        var item = Manager<ItemBase>.pic[nameDisplay];

        return SetButtonA(item.nameDisplay, item.image, item.nameDisplay, null);
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
        myItem = Manager<ItemBase>.pic[itemName].Create();

        textButton.text = itemName;

        previewImage.sprite = sprite;

        myNum.text = textNum;

        if (action != null)
        {
            listeners += action;
            empty = false;
        }

        return this;
    }

}
