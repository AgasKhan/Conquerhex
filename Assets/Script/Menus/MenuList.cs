using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuList : PopUp
{

    List<DoubleString> defaultOptions = new List<DoubleString>();

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

    private void OnDisable()
    {
        buttonFactory.DestroyAll();
    }
}

[System.Serializable]
public class ButtonFactory
{
    public EventsCall prefab;

    public List<EventsCall> eventsCalls = new List<EventsCall>();

    public float durationWait = 0.5f;

    public Transform content;

    public void Create(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);
    }

    public ButtonFactory DestroyAll()
    {
        for (int i = 0; i < eventsCalls.Count; i++)
        {
            Object.Destroy(eventsCalls[i].gameObject);
        }

        eventsCalls.Clear();

        return this;
    }   
}