using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LisNavBarModule : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI title;

    [SerializeField]
    ButtonFactory navbar;

    [SerializeField]
    TextMeshProUGUI[] tags;

    [SerializeField]
    Transform buttonsContent;

    [SerializeField]
    ButtonHor buttonHor;

    public LisNavBarModule AddNavBarButton(string text, string buttonName)
    {
        return AddNavbarButton(text, buttonName, null);
    }

    public LisNavBarModule AddNavBarButton(string text, UnityEngine.Events.UnityAction action)
    {
        return AddNavbarButton(text, "", action);
    }

    LisNavBarModule AddNavbarButton(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        UnityEngine.Events.UnityAction aux = action;

        action = () => title.text = text;

        action += aux;

        navbar.Create(text, buttonName, action);

        return this;
    }

    public ButtonHor AddButtonHor(Sprite _image, string _name, string[] _tags, UnityEngine.Events.UnityAction _action)
    {
        var aux = Object.Instantiate(buttonHor, buttonsContent);

        return aux.SetButton(_image, _name, _tags, _action);
    }

    public LisNavBarModule SetTitle(string _title)
    {
        title.text = _title;
        return this;
    }

    public LisNavBarModule SetTags(string[] _tags)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            tags[i].text = _tags[i];
        }
        return this;
    }
}
