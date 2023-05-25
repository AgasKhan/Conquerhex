using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    string firstLevel;

    public AudioManager audioM;
    public AudioMixerGroup effects;
    public AudioMixerGroup music;

    //para los eventos-------------------------------------------------------
    public Pictionarys<string, Action<GameObject>> eventListVoid = new Pictionarys<string, Action<GameObject>>();

    public Pictionarys<string, Action<Button>> eventListButtoOn = new Pictionarys<string, Action<Button>>();
    //-----------------------------------------------------------------------

    //[SerializeField]
    public Pictionarys<string, DetailsWindow> detailsWindows;


    //se debe eliminar este
    [SerializeField]
    public ManagerSubMenus subMenus;

    [SerializeField]
    public ManagerModulesMenu modulesMenu;


    protected override void Awake()
    {
        base.Awake();

        audioM = GetComponent<AudioManager>();

        GetDetailsWinAndSubMenus();

        LoadSystem.AddPostLoadCorutine(InitScenes);

        subMenus.Init();
    }

    void GetDetailsWinAndSubMenus()
    {
        var allDetailsWindows = GetComponentsInChildren<DetailsWindow>();

        foreach (var item in allDetailsWindows)
        {
            detailsWindows.Add(item.name, item);
        }
    }

    void InitScenes()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            MusicInMenu(true);
        }
        else
        {
            GameMusic(true);
            FirstStart();
        }

        if (SaveWithJSON.CheckKeyInBD("MusicVolume"))
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("MusicVolume"), "Music");
        else
            ChangeVolume(1f, "Music");

        if (SaveWithJSON.CheckKeyInBD("EffectsVolume"))
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("EffectsVolume"), "Effects");
        else
            ChangeVolume(1f, "Effects");
    }

    void FirstStart()
    {
        if(!SaveWithJSON.CheckKeyInBD("ShowTutorial"))
        {
            SaveWithJSON.SaveInPictionary("ShowTutorial", false);
            subMenus.ShowWindow("ShowControls");
        }
    }

    public void StartGame()
    {
        //ClickAccept();
        LoadSystem.instance.Load(firstLevel, true);
    }

    public void MuteCurrentMusic(bool condition)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            MusicInMenu(condition);
        else
            GameMusic(condition);
    }

    public void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;

        if (name == "Music")
        {
            music.audioMixer.SetFloat(name, value);
            SaveWithJSON.SaveInPictionary("MusicVolume", volume);
        }
        else
        {
            effects.audioMixer.SetFloat(name, value);
            SaveWithJSON.SaveInPictionary("EffectsVolume", volume);
        }

    }



    public void ClickSound()
    {
        audioM.Play("Click");
    }
    public void MusicInMenu(bool condition)
    {
        if(condition == true)
            audioM.Play("MusicInMenu");
        else
            audioM.Pause("MusicInMenu");
    }

    public void GameMusic(bool condition)
    {
        if (condition == true)
            audioM.Play("GameMusic");
        else
            audioM.Pause("GameMusic");
    }

    public void StartSound()
    {
        audioM.Play("MusicInMenu");
    }

    public void PlayTutorialM(bool condition)
    {
        if (condition == true)
            audioM.Play("TutorialMusic");
        else
            audioM.Pause("TutorialMusic");
    }

    /*
     * Sonidos
    public void ClickSound()
    {
        audioM.Play("Click");
    }

    public void ClickAccept()
    {
        audioM.Play("ClickAccept");
    }

    public void ClaimSound()
    {
        audioM.Play("Claim");
    }

    public void FireworksSound()
    {
        audioM.Play("Fireworks");
    }
    */

    private void OnDestroy()
    {
        detailsWindows.Clear();
    }
}


[System.Serializable]
public struct DoubleString
{
    [TextArea(1, 6)]
    public string superior;

    [TextArea(3, 6)]
    public string inferior;

    public DoubleString(string superior, string inferior)
    {
        this.superior = superior;
        this.inferior = inferior;
    }
}



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
        for (int i = 0; i < reference.childCount; i++)
        {
            subMenus.Add(reference.GetChild(i).gameObject);
        }

        Manager<ManagerSubMenus>.pic.Add(reference.name, this);
    }
}



