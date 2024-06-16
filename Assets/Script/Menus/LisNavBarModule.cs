using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LisNavBarModule : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    GameObject NavBarContent;

    [SerializeField]
    TextMeshProUGUI[] Tags;

    [SerializeField]
    GameObject ButtonsContent;

    [SerializeField]
    ButtonFactory navbar;


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

        action = () => Title.text = text;

        action += aux;

        navbar.Create(text, buttonName, action);

        return this;
    }

}
