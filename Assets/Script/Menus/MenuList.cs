using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuList : MonoBehaviour
{
    [SerializeField]
    Transform content;

    public EventsCall prefab;

    public float durationWait = 0.5f;

    List<StringAction> defaultOptions = new List<StringAction>();

    List<EventsCall> eventsCalls = new List<EventsCall>();

    public MenuList CreateDefault()
    {
        CreateConfigured(defaultOptions.ToArray());

        return this;
    }

    public MenuList CreateConfigured(params StringAction[] stringActions)
    {
        foreach (var item in stringActions)
        {
            Create(item.text, item.buttonName);
        }

        return this;
    }

    public MenuList Create(params StringAction[] stringActions)
    {
        foreach (var item in stringActions)
        {
            Create(item.text, item.action);
        }

        return this;
    }

    public MenuList Create(Pictionarys<string, LogicActive> pictionaries)
    {
        foreach (var item in pictionaries)
        {
            Create(item.key, item.value.Activate);
        }

        return this;
    }

    public MenuList Create(string text, string buttonName)
    {
        Create(text, buttonName, null);
        return this;
    }

    public MenuList Create(string text, UnityEngine.Events.UnityAction action)
    {
        Create(text, "", action);
        return this;
    }

    public MenuList DestroyAll()
    {
        for (int i = 0; i < eventsCalls.Count; i++)
        {
            Destroy(eventsCalls[i].gameObject);
        }

        eventsCalls.Clear();

        return this;
    }

    void Create(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        eventsCalls[eventsCalls.Count - 1].fadeMenu.OnFade().Set(eventsCalls.Count * durationWait);
    }

    private void Awake()
    {
        eventsCalls.AddRange(content.GetComponentsInChildren<EventsCall>());

        foreach (var item in eventsCalls)
        {
            item.button.Event();

            //crear remplazo vacio

            defaultOptions.Add(new StringAction(item.textButton.text, item.button.name));
        }

        DestroyAll();
    }

    private void OnDisable()
    {
        DestroyAll();
    }
}


[System.Serializable]
public struct StringAction
{
    public string text;

    public string buttonName;

    public UnityEngine.Events.UnityAction action;

    public StringAction(string text, UnityAction action)
    {
        this.text = text;
        this.action = action;
        buttonName = "";
    }

    public StringAction(string text, string buttonName)
    {
        this.text = text;
        this.action = null;
        this.buttonName = buttonName;
    }
}