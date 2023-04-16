using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInformation : MonoBehaviour
{
    public ButtonData buttonData;

    private void Start()
    {
        var aux = DetailsWindowsManager.instance.detailsWindows[transform.name];

        if (aux != null)
            buttonData.detailWindow = aux;
    }
}
