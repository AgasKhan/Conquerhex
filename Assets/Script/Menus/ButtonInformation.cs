using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonInformation : MonoBehaviour
{
    public ItemBase myDetails;

    [HideInInspector]
    public DetailsWindow myDetailWindow;

    private void Start()
    {
        var aux = Manager<DetailsWindow>.pic[transform.name];

        if (aux != null)
            myDetailWindow = aux;
        else
            Debug.Log("No se encontro: " + transform.name + " entre las Details Windows");
    }
}
