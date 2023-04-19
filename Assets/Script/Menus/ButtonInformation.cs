using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInformation : MonoBehaviour
{
    public ItemBase myItem;

    [HideInInspector]
    public DetailsWindow myDetailWindow;

    private void Start()
    {
        if (myItem != null)
            GetDetailsWindow();
    }

    public void GetDetailsWindow()
    {
        if (MenuManager.instance.detailsWindows.ContainsKey(transform.name))
            myDetailWindow = MenuManager.instance.detailsWindows[transform.name];
        else
            Debug.Log("No se encontro: " + transform.name + " entre las Details Windows");
    }

}
