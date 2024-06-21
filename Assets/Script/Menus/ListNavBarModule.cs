using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ListNavBarModule : MonoBehaviour
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

    [SerializeField]
    RectTransform listTransform;

    [SerializeField]
    GameObject auxiliarButtons;

    [SerializeField]
    EventsCall buttonLeft;

    [SerializeField]
    EventsCall buttonRight;

    [SerializeField]
    Image arrow;

    List<ButtonHor> buttonsList = new List<ButtonHor>();

    public ListNavBarModule AddNavBarButton(string text, string buttonName)
    {
        return AddNavbarButton(text, buttonName, null);
    }

    public ListNavBarModule AddNavBarButton(string text, UnityEngine.Events.UnityAction action)
    {
        return AddNavbarButton(text, "", action);
    }

    public ButtonHor AddButtonHor(string _name, Sprite _image, ItemTags _tags, UnityEngine.Events.UnityAction _action)
    {
        var aux = Object.Instantiate(buttonHor, buttonsContent);
        buttonsList.Add(aux);

        if (_image == null)
            aux.previewImage.SetActive(false);

        return aux.SetButton(_image, _name, _tags, _action);
    }

    public void ClearButtonsHor()
    {
        foreach (var item in buttonsList)
        {
            Object.Destroy(item.gameObject);
        }
        buttonsList.Clear();
    }

    public ListNavBarModule SetTitle(string _title)
    {
        title.text = _title;
        return this;
    }

    public ListNavBarModule SetTags(ItemTags _tags)
    {
        tags[0].text = _tags.tagOne;
        tags[1].text = _tags.tagTwo;
        tags[2].text = _tags.tagThree;
        tags[3].text = _tags.tagFour;

        return this;
    }

    public EventsCall SetLeftAuxButton(string text, UnityEngine.Events.UnityAction action, string buttonName)
    {
        ShowHideAuxButtons(true);
        action += () => SetArrow(true);
        buttonLeft.Set(text, action, buttonName);
        return buttonLeft;
    }

    public EventsCall SetRightAuxButton(string text, UnityEngine.Events.UnityAction action, string buttonName)
    {
        ShowHideAuxButtons(true);
        action += () => SetArrow(false);
        arrow.SetActiveGameObject(true);
        buttonRight.SetActiveGameObject(true);
        buttonRight.Set(text, action, buttonName);
        return buttonRight;
    }

    public void SetArrow(bool value)
    {
        arrow.transform.localScale = value? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        arrow.SetActiveGameObject(true);
    }

    public void ShowHideAuxButtons(bool value)
    {
        auxiliarButtons.transform.SetActiveGameObject(value);

        if(value)
            listTransform.sizeDelta = new Vector2(840, 670);
        else
            listTransform.sizeDelta = new Vector2(840, 780);
    }

    ListNavBarModule AddNavbarButton(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        UnityEngine.Events.UnityAction aux = action;

        action = () => title.text = text;

        action += aux;

        navbar.Create(text, buttonName, action);

        return this;
    }

    private void OnDisable()
    {
        ShowHideAuxButtons(false);
        buttonRight.SetActiveGameObject(false);
        SetArrow(true);
        arrow.SetActiveGameObject(false);
        ClearButtonsHor();
    }
}

public struct ItemTags
{
    public string tagOne;
    public string tagTwo;
    public string tagThree;
    public string tagFour;

    public ItemTags(string tagOne, string tagTwo, string tagThree, string tagFour)
    {
        this.tagOne = tagOne;
        this.tagTwo = tagTwo;
        this.tagThree = tagThree;
        this.tagFour = tagFour;
    }
}