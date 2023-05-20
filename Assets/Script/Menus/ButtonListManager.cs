using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListManager : MonoBehaviour
{
    [SerializeField]
    Transform content;

    public EventsCall prefab;

    public void Create(Sprite sprite, UnityEngine.Events.UnityAction action = null)
    {
        var aux = Instantiate(prefab, content);

        aux.image.sprite = sprite;

        if(action!=null)
            aux.listeners += action;
    }

    public void DestroyAll()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(0).gameObject);
        }
    }
}
