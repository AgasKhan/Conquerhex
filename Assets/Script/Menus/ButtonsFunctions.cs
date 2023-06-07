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
            {"MuteEffects", MuteEffects},
            {"MuteMusic", MuteMusic},

            //Manejo de ventanas
            {"PopUp", PopUp},
            {"ShowWindow", ShowWindow},
            {"CloseWindow", CloseWindow},

        });
    }

   
    #region Static Buttons
    protected void Example(GameObject g)
    {
        //Debug.Log("Apretaste el boton");
    }

    protected void DisplayWindow(GameObject g)
    {
        //refMenu.subMenus.ShowWindow(g.name);
    }

    protected void MuteEffects(GameObject g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = g.transform.GetComponent<UnityEngine.UI.Image>();

        if (textChild.text == "ON")
        {
            refMenu.ChangeVolume(0, "Effects");
            textChild.text = "MUTE";
            imageChild.color = Color.gray;
        }
        else
        {
            refMenu.ChangeVolume(1f, "Effects");
            textChild.text = "ON";
            imageChild.color = Color.white;
        }
    }
    protected void MuteMusic(GameObject g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = g.transform.GetComponent<UnityEngine.UI.Image>();

        if (textChild.text == "ON")
        {
            refMenu.ChangeVolume(0, "Music");
            textChild.text = "MUTE";
            imageChild.color = Color.gray;
            refMenu.MuteCurrentMusic(false);
        }
        else
        {
            refMenu.ChangeVolume(1f, "Music");
            textChild.text = "ON";
            imageChild.color = Color.white;
            refMenu.MuteCurrentMusic(true);
        }
    }

    protected void Restart(GameObject g)
    {
        LoadSystem.instance.Reload();
        //refMenu.subMenus.CloseLastWindow();
    }

    /*
    void ShowControls(GameObject g)
    {
        var myImage = g.transform.GetComponent<UnityEngine.UI.Image>();

        var tempColor = myImage.color;
        tempColor.a = 1f;

        var tempColor2 = myImage.color;
        tempColor2.a = 0.49f;

        if (myImage.color.a == 1f)
        {
            myImage.color = tempColor2;
        }
        else
        {
            myImage.color = tempColor;
        }

    }
    */

    protected void PopUp(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("hola", "Mundo");
    }

    void ShowWindow(GameObject g)
    {
        Debug.Log("Apretaste un boton Draggeable");
        //DetailsWindow.instance.SetWindow(_mySprite, _information);
    }

    protected void CloseWindow(GameObject g)
    {
        //refMenu.subMenus.CloseLastWindow();
    }

    #endregion
}
