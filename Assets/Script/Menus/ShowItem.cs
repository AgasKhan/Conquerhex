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
        if (Manager<DetailsWindow>.pic.ContainsKey(transform.name))
            myDetailWindow = Manager<DetailsWindow>.pic[transform.name];
        else
            Debug.Log("No se encontro: " + transform.name + " entre las Details Windows");
    }

    protected override void InternalActivate(params Button[] specificParam)
    {
        //specificParam[0]
        if (Manager<WeaponBase>.pic.ContainsKey(transform.parent.name))
        {
            myItem = Manager<WeaponBase>.pic[transform.parent.name];
            myDetailWindow.SetWindow(myItem.image, myItem.nameDisplay, ((IShowItem)myItem).details.ToString(" = ", "\n \n"));
        }
        else
            Debug.Log("No se encontro el item: " + transform.parent.name);
        
    }
}
