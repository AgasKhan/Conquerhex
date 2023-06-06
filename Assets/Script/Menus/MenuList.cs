using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuList : PopUp
{

    List<DoubleString> defaultOptions = new List<DoubleString>();

    public override PopUp SetWindow(string titulo, string text, Sprite sprite = null)
    {
        detailsWindow.transform.parent.gameObject.SetActive(true);
        return base.SetWindow(titulo, text, sprite);
    }

    public MenuList CreateConfigured(params DoubleString[] stringActions)
    {
        foreach (var item in stringActions)
        {
            AddButton(item.superior, item.inferior);
        }

        return this;
    }

    public MenuList CreateDefault()
    {
        return CreateConfigured(defaultOptions.ToArray()); ;
    }

    public MenuList AddButton(Pictionarys<string, LogicActive> pictionaries)
    {
        foreach (var item in pictionaries)
        {
            AddButton(item.key, item.value.Activate);
        }

        return this;
    }

    private void Awake()
    {
        buttonFactory.eventsCalls.AddRange(buttonFactory.content.GetComponentsInChildren<EventsCall>());

        foreach (var item in buttonFactory.eventsCalls)
        {
            item.button.Event();

            //crear remplazo vacio

            defaultOptions.Add(new DoubleString(item.textButton.text, item.button.name));
        }

        buttonFactory.DestroyAll();
    }

    private void OnEnable()
    {
        detailsWindow.transform.parent.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        buttonFactory.DestroyAll();
    }
}