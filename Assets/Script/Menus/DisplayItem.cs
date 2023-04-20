using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem : LogicActive<UnityEngine.UI.Button>
{
    public ItemBase myItem;

    [HideInInspector]
    public DetailsWindow myDetailWindow;

    private void Start()
    {
        GetDetailsWindow();
    }

    public void GetDetailsWindow()
    {
        if (Manager<DetailsWindow>.pic.ContainsKey(transform.name))
            myDetailWindow = Manager<DetailsWindow>.pic[transform.name];
        else
            Debug.Log("No se encontro: " + transform.name + " entre las Details Windows");
    }

    protected override void InternalActivate(params Button[] specificParam)
    {
        //specificParam[0]
        if (Manager<AbilityBase>.pic.ContainsKey(transform.parent.name))
        {
            myItem = Manager<AbilityBase>.pic[transform.parent.name];
            myDetailWindow.SetWindow(myItem.image, myItem.nameDisplay, myItem.GetDetails().ToString("\n", "\n \n"));
        }
        else
            Debug.Log("No se encontro el item: " + transform.parent.name);
        
    }
}
