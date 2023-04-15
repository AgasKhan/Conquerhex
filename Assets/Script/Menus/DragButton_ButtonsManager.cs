using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragButton_ButtonsManager : ButtonsManager
{
    [SerializeField]
    Sprite _mySprite;

    [SerializeField]
    DoubleString _information;


    private void Start()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"ShowWindow", ShowWindow}

        });
    }

    void ShowWindow(GameObject g)
    {
        Debug.Log("Apretaste un boton Draggeable");
        //DetailsWindow.instance.SetWindow(_mySprite, _information);
    }
}
