using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    string firstLevel;

    [SerializeField]
    ShowSubMenuSettings showSubMenuSettings;


    //para los eventos-------------------------------------------------------
    public Pictionarys<string, Action<GameObject>> eventListVoid = new Pictionarys<string, Action<GameObject>>();

    public Pictionarys<string, Action<Button>> eventListButtoOn = new Pictionarys<string, Action<Button>>();
    //-----------------------------------------------------------------------

    [SerializeField]
    public ManagerModulesMenu modulesMenu;

    protected override void Awake()
    {
        base.Awake();

        showSubMenuSettings.Init(GetComponent<AudioManager>());

        LoadSystem.AddPostLoadCorutine(showSubMenuSettings.InitScenes);
    }

    



    public void StartGame()
    {
        //ClickAccept();
        LoadSystem.instance.Load(firstLevel, true);
    }
}


[System.Serializable]
public struct DoubleString
{
    [TextArea(1, 6)]
    public string superior;

    [TextArea(3, 6)]
    public string inferior;

    /// <summary>
    /// Este es el constructor de la estructura DoubleString. Recibe dos cadenas de texto como parámetros: superior y inferior.
    /// </summary>
    /// <param name="superior"></param>
    /// <param name="inferior"></param>
    public DoubleString(string superior, string inferior)
    {
        this.superior = superior;
        this.inferior = inferior;
    }
}



/// <summary>
/// version antigua de menu, previa al submenu por componente
/// </summary>
[System.Serializable]
public class ManagerSubMenus : Init
{
    /// <summary>
    /// Variable que contiene el transform que almacena todos los submenus
    /// </summary>
    public Transform reference;

    public List<GameObject> subMenus = new List<GameObject>();
    public Transform LastWindow
    {
        get
        {
            return reference.GetChild(reference.childCount - 1);
        }
    }

    public void ShowWindow(string key)
    {
        Open();

        foreach (var item in subMenus)
        {
            if (item.name == key)
            {
                item.gameObject.SetActive(true);
                item.transform.SetAsLastSibling();
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        } 
    }

    public void CloseLastWindow()
    {
        LastWindow.gameObject.SetActive(false);

        LastWindow.SetAsFirstSibling();
    }

    public void PreviusWindow()
    {
        CloseLastWindow();

        LastWindow.gameObject.SetActive(true);
    }

    public void Close()
    {
        reference.gameObject.SetActive(false);
    }

    public void Open()
    {
        reference.gameObject.SetActive(true);
    }

    /// <summary>
    /// El primer parametro debe de ser el transform que contenga los submenus
    /// </summary>
    /// <param name="param"></param>
    public void Init(params object[] param)
    {
        if (reference == null)
            return;

        for (int i = 0; i < reference.childCount; i++)
        {
            subMenus.Add(reference.GetChild(i).gameObject);
        }
        
           

        Manager<ManagerSubMenus>.pic.Add(reference.name, this);
    }
}



