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

    public ButtonHor SetButton(Sprite _image, string _name, string[] _tags, UnityEngine.Events.UnityAction _action)
    {
        previewImage.sprite = _image;
        itemName.text = _name;

        if(_tags!=null)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i].text = _tags[i];
            }
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(_action);

        return this;
    }

    public ButtonHor SetType(string type)
    {
        this.type = type;

        return this;
    }
}
