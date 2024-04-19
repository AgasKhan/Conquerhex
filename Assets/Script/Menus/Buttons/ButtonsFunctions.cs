using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFunctions : MonoBehaviour
{
    protected MenuManager refMenu;

    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(LoadButtons);
    }
    private void Start()
    {
        refMenu = MenuManager.instance;
    }

    protected virtual void LoadButtons()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            // Accion por defecto
            {"Button", Example},
            
            //Opciones
            {"Settings", DisplayWindow},
            //Manejo de ventanas
            {"PopUp", PopUp},
            {"ShowWindow", ShowWindow},
            {"CloseWindow", CloseWindow},
            {"ExitSubMenu", ExitSubMenu}

        });
    }

   
    #region Static Buttons
    void Example(GameObject g)
    {
        //Debug.Log("Apretaste el boton");
    }

    void DisplayWindow(GameObject g)
    {
        //refMenu.subMenus.ShowWindow(g.name);
    }


    protected void Restart(GameObject g)
    {
        LoadSystem.instance.Reload();
        //refMenu.subMenus.CloseLastWindow();
    }


    void PopUp(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("hola", "Mundo");
    }

    void ShowWindow(GameObject g)
    {
        Debug.Log("Apretaste un boton Draggeable");
        //DetailsWindow.instance.SetWindow(_mySprite, _information);
    }

    void ExitSubMenu(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<SubMenus>().TriggerOnClose();
    }

    protected void CloseWindow(GameObject g)
    {
        //refMenu.subMenus.CloseLastWindow();
    }

    #endregion
}
