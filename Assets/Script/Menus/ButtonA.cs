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

        // Configuracion del bot�n 
        button = buttonEventsCall.button;
        textButton = buttonEventsCall.textButton;
        fadeMenu = buttonEventsCall.fadeMenu;

        textButton.text = text;

        return this;
    }

    public ButtonA CloneA(string buttonText, Sprite sprite, string otherText, UnityEngine.Events.UnityAction action, string buttonName, Transform content)
    {
        //Crea una instancia de EventsCall usando Instantiate y establece su posici�n en content
        var aux = Instantiate(this, content);

        //asigno el texto al campo textButton.text
        aux.textButton.text = buttonText;

        previewImage.sprite = sprite;

        myNum.text = otherText;

        // Si el nombre del bot�n no est� vac�o, establece el nombre del bot�n en buttonName
        // Si el nombre del bot�n est� vac�o, cambia el estado del primer listener del evento onClick del bot�n a Off
        if (buttonName != "")
            aux.button.name = buttonName;
        else
        {
            aux.button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        //Si la acci�n no es nula, agrega la acci�n al evento listeners y establece empty en false
        if (action != null)
        {
            aux.listeners += action;
            aux.empty = false;
        }

        return aux;
    }

}
