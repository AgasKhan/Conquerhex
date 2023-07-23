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
    /// Crea y configura un nuevo bot�n
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

        //Se crea una nueva instancia de bot�n utilizando el m�todo Clone del objeto prefab y se agrega a la lista eventsCalls.
        eventsCalls.Add(prefab.Clone(text, action, buttonName, content));

        //Se activa una animaci�n dentro del menu del bot�n reci�n creado utilizando el obj fadeMenu. La duraci�n se establece en funci�n del n�mero de botones existentes en eventsCalls y el valor de durationWait.
        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        //Si este es el primer bot�n creado y firstOpaque da true se desactiva la interacci�n del bot�n para que no se pueda hacer clic en �l.
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
