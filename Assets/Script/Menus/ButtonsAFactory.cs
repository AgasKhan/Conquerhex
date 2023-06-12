using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class ButtonsAFactory : ButtonFactory
{
    public ButtonA newPrefab;

    public ButtonFactory CreateAButtons(string text, string buttonName, Sprite sprite, string otherText, UnityEngine.Events.UnityAction action)
    {
        if (selectOpaque)
            action += () =>
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

        //Se crea una nueva instancia de botón utilizando el método Clone del objeto prefab y se agrega a la lista eventsCalls.
        eventsCalls.Add(newPrefab.CloneA(text, sprite, otherText, action, buttonName, content));

        //Se activa una animación dentro del menu del botón recién creado utilizando el obj fadeMenu. La duración se establece en función del número de botones existentes en eventsCalls y el valor de durationWait.
        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        //Si este es el primer botón creado y firstOpaque da true se desactiva la interacción del botón para que no se pueda hacer clic en él.
        if (eventsCalls.Count == 1 && firstOpaque)
        {
            eventsCalls[eventsCalls.Count - 1].button.interactable = false;
        }

        return this;
    }
}
