using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DisplayItem : LogicActive<UnityEngine.UI.Button>
{
    public IShowDetails myItem;

    public string specialAction = "";

    [HideInInspector]
    public DetailsWindow myDetailWindow;

    void GetVariables()
    {
        myItem = GetScriptObject(transform.parent.name);
        myDetailWindow = Manager<DetailsWindow>.pic[transform.name];
    }

    IShowDetails GetScriptObject(string ObjectName)
    {
        if (Manager<ShowDetails>.pic.ContainsKey(ObjectName))
            return Manager<ShowDetails>.pic[transform.parent.name];
        else
            return Manager<ItemBase>.pic[transform.parent.name];
    }

    protected override void InternalActivate(params Button[] specificParam)
    {
        GetVariables();
        if (myItem == null || myDetailWindow == null)
        {
            Debug.LogWarning("No se encontro un parametro necesario");
            return;
        }

        myDetailWindow.SetWindow(myItem.image, myItem.nameDisplay, myItem.GetDetails().ToString("\n", "\n \n"));
    }
}
