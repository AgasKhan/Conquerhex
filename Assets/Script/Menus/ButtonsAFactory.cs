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

        //Se crea una nueva instancia de bot�n utilizando el m�todo Clone del objeto prefab y se agrega a la lista eventsCalls.
        eventsCalls.Add(newPrefab.CloneA(text, sprite, otherText, action, buttonName, content));

        //Se activa una animaci�n dentro del menu del bot�n reci�n creado utilizando el obj fadeMenu. La duraci�n se establece en funci�n del n�mero de botones existentes en eventsCalls y el valor de durationWait.
        eventsCalls[eventsCalls.Count - 1].fadeMenu.FadeOn().Set(eventsCalls.Count * durationWait);

        //Si este es el primer bot�n creado y firstOpaque da true se desactiva la interacci�n del bot�n para que no se pueda hacer clic en �l.
        if (eventsCalls.Count == 1 && firstOpaque)
        {
            eventsCalls[eventsCalls.Count - 1].button.interactable = false;
        }

        return this;
    }
}
