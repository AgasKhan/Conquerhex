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

    /// <summary>
    /// Crea y configura un nuevo botón
    /// </summary>
    /// <param name="text"></param>
    /// <param name="buttonName"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public ButtonFactory Create(string text, string buttonName, UnityEngine.Events.UnityAction action)
    {
        if(selectOpaque)
        {
            UnityEngine.Events.UnityAction aux = action;

            action = () =>
            {
                foreach (var item in eventsCalls)
                {
                    if (EventSystem.current.currentSelectedGameObject == item.button.gameObject)
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

            action += aux;
        }

        //Se crea una nueva instancia de botón utilizando el método Clone del objeto prefab y se agrega a la lista eventsCalls.
        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        //Se activa una animación dentro del menu del botón recién creado utilizando el obj fadeMenu. La duración se establece en función del número de botones existentes en eventsCalls y el valor de durationWait.
        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        //Si este es el primer botón creado y firstOpaque da true se desactiva la interacción del botón para que no se pueda hacer clic en él.
        if (eventsCalls.Count==1 && firstOpaque)
        {
            eventsCalls[0].button.interactable = false;
        }

        return this;
    }

    /// <summary>
    /// Destruye y elimina todos los botones creados por ButtonFactory.
    /// </summary>
    /// <returns></returns>
    public ButtonFactory DestroyAll()
    {
        //recorre la lista eventsCalls y se destruyen todos los objetos gameObject asociados a los botones
        for (int i = 0; i < eventsCalls.Count; i++)
        {
            Object.Destroy(eventsCalls[i].gameObject);
        }

        //Limpia el eventsCalls
        eventsCalls.Clear();

        return this;
    }
}
