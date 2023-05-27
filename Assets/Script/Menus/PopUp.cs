using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField]
    protected DetailsWindow detailsWindow;

    [SerializeField]
    protected ButtonFactory buttonFactory;

    public virtual PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        detailsWindow.SetTexts(titulo, text).SetImage(sprite);
        buttonFactory.DestroyAll();
        buttonFactory.content.gameObject.SetActive(false);
        return this;
    }

    public PopUp AddButton(string text, string buttonName)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, buttonName, null);
        return this;
    }

    public PopUp AddButton(string text, UnityEngine.Events.UnityAction action)
    {
        buttonFactory.content.gameObject.SetActive(true);
        buttonFactory.Create(text, "", action);
        return this;
    }

}
