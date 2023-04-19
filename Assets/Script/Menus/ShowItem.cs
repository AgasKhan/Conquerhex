using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItem : LogicActive<UnityEngine.UI.Button>
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

    protected override void InternalActivate(params Button[] specificParam)
    {
        //specificParam[0]

        

        myDetailWindow.SetWindow(myItem.image, myItem.nameDisplay, myItem.details.ToString(" = ", "\n \n"));
    }
}
