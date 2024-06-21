using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonHor : MonoBehaviour
{
    [SerializeField]
    public Image previewImage;

    [SerializeField]
    public TextMeshProUGUI itemName;

    [SerializeField]
    public TextMeshProUGUI[] tags;

    public string type;

    [SerializeField]
    Button myButton;

    public ButtonHor SetButton(Sprite _image, string _name, ItemTags _tags, UnityEngine.Events.UnityAction _action)
    {
        previewImage.sprite = _image;
        itemName.text = _name;

        SetTags(_tags);

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(_action);

        return this;
    }

    public ButtonHor SetTags(ItemTags _tags)
    {
        tags[0].text = _tags.tagOne;
        tags[1].text = _tags.tagTwo;
        tags[2].text = _tags.tagThree;
        tags[3].text = _tags.tagFour;

        return this;
    }

    public ButtonHor SetType(string type)
    {
        this.type = type;

        return this;
    }
}
