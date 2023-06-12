using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonA : EventsCall
{
    [SerializeField]
    Image previewImage;

    [SerializeField]
    TextMeshProUGUI myNum;

    public ButtonA SetButtonA(Sprite sprite, EventsCall buttonEventsCall, string text)
    {
        previewImage.sprite = sprite;

        // Configuracion del botón 
        button = buttonEventsCall.button;
        textButton = buttonEventsCall.textButton;
        fadeMenu = buttonEventsCall.fadeMenu;

        textButton.text = text;

        return this;
    }

    public ButtonA CloneA(string buttonText, Sprite sprite, string otherText, UnityEngine.Events.UnityAction action, string buttonName, Transform content)
    {
        //Crea una instancia de EventsCall usando Instantiate y establece su posición en content
        var aux = Instantiate(this, content);

        //asigno el texto al campo textButton.text
        aux.textButton.text = buttonText;

        previewImage.sprite = sprite;

        myNum.text = otherText;

        // Si el nombre del botón no está vacío, establece el nombre del botón en buttonName
        // Si el nombre del botón está vacío, cambia el estado del primer listener del evento onClick del botón a Off
        if (buttonName != "")
            aux.button.name = buttonName;
        else
        {
            aux.button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        //Si la acción no es nula, agrega la acción al evento listeners y establece empty en false
        if (action != null)
        {
            aux.listeners += action;
            aux.empty = false;
        }

        return aux;
    }

}
