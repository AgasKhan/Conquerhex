using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonFactory
{
    public EventsCall prefab;

    public List<EventsCall> eventsCalls = new List<EventsCall>();

    public float durationWait = 0.5f;

    public Transform content;

    public ButtonFactory Create(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        return this;
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
