using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class ButtonFactory
{
    public bool selectOpaque;

    public bool firstOpaque;

    public EventsCall prefab;

    public List<EventsCall> eventsCalls = new List<EventsCall>();

    public float durationWait = 0.5f;

    public Transform content;

    public ButtonFactory Create(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        if(selectOpaque)
            action += () =>
             {
                 foreach (var item in eventsCalls)
                 {
                     if(EventSystem.current.currentSelectedGameObject == item.button.gameObject)
                     {
                         item.button.interactable = false;
                     }
                     else
                     {
                         item.button.interactable = true;
                     }
                 }
             }
             ;

        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        if(eventsCalls.Count==1 && firstOpaque)
        {
            eventsCalls[eventsCalls.Count - 1].button.interactable = false;
        }

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
