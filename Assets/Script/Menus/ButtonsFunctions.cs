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
            {"AddEffectsVol", AddEffectsVol},
            {"SubsEffectsVol", SubsEffectsVol},
            
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

    void MuteEffects(GameObject g)
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

    void AddEffectsVol(GameObject g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("EffectsVolume") + 5f;

        if (aux > 100)
            aux = 100;

        refMenu.ChangeVolume(aux, "EffectsVolume");
    }

    void SubsEffectsVol(GameObject g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("EffectsVolume") - 5f;

        if (aux < 0)
            aux = 0;

        refMenu.ChangeVolume(aux, "EffectsVolume");
    }


    void AddMusicVol(GameObject g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("MusicVolume") + 5f;

        if (aux > 100)
            aux = 100;

        refMenu.ChangeVolume(aux, "MusicVolume");
    }

    void SubsMusicVol(GameObject g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("MusicVolume") - 5f;

        if (aux < 0)
            aux = 0;

        refMenu.ChangeVolume(aux, "MusicVolume");
    }
    
    void MuteMusic(GameObject g)
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
        refMenu.modulesMenu.ObtainMenu<SubMenus>(false);
    }

    protected void CloseWindow(GameObject g)
    {
        //refMenu.subMenus.CloseLastWindow();
    }

    #endregion
}
