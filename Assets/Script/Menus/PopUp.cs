using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField]
    DetailsWindow detailsWindow;

    [SerializeField]
    MenuList menuList;

    public PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        detailsWindow.SetWindow(sprite, titulo, text);
        menuList.DestroyAll();
        menuList.SetActiveGameObject(false);
        return this;
    }

    public PopUp AddButton(string text, string buttonName)
    {
        menuList.SetActiveGameObject(true);
        menuList.Create(text, buttonName);
        return this;
    }

    public PopUp AddButton(string text, UnityEngine.Events.UnityAction action)
    {
        menuList.SetActiveGameObject(true);
        menuList.Create(text, action);
        return this;
    }

}
