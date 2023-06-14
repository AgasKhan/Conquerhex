using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class ShowSubMenuSettings : CreateSubMenu
{

    public AudioMixerGroup music;
    public AudioManager audioM;
    public AudioMixerGroup effects;

    public override void Init(params object[] param)
    {
        base.Init(param);

        audioM = param[0] as AudioManager;
    }

    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }

    protected override void InternalCreate()
    {
        CreateNavBar((subMenus)=>
        {
            subMenus.AddNavBarButton("Sounds","Settings");
        
        });

        subMenu.CreateSection(0, 3);
 
            subMenu.AddComponent<DetailsWindow>().SetTexts("Music Volumen","");
                
            subMenu.AddComponent<ButtonB>();

        subMenu.CreateSection(3, 6);

            subMenu.AddComponent<DetailsWindow>().SetTexts("Effects Volumen", "");

            subMenu.AddComponent<ButtonB>();


        if (SaveWithJSON.CheckKeyInBD("MusicVolume"))
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("MusicVolume"), "Music");
        else
            ChangeVolume(1f, "Music");

        if (SaveWithJSON.CheckKeyInBD("EffectsVolume"))
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("EffectsVolume"), "Effects");
        else
            ChangeVolume(1f, "Effects");
    }


    public void InitScenes()
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

    }

    void FirstStart()
    {
        if (!SaveWithJSON.CheckKeyInBD("ShowTutorial"))
        {
            SaveWithJSON.SaveInPictionary("ShowTutorial", false);
            //subMenus.ShowWindow("ShowControls");
            //aqui se debe abrir el mostrar controles
        }
    }


    public void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;

        music.audioMixer.SetFloat(name, value);
        SaveWithJSON.SaveInPictionary(name, volume);

  
    }

    public void MuteCurrentMusic(bool condition)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            MusicInMenu(condition);
        else
            GameMusic(condition);
    }

    public void MusicInMenu(bool condition)
    {
        if (condition == true)
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

    //////////////////////////////////buttonFunctions////////
    void MuteMusic(Image g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        if (textChild.text == "ON")
        {
            ChangeVolume(0, "Music");
            textChild.text = "MUTE";
            MuteCurrentMusic(false);
        }
        else
        {
            ChangeVolume(1f, "Music");
            textChild.text = "ON";
            MuteCurrentMusic(true);
        }
    }
    void MuteEffects(Image g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        
        if (textChild.text == "ON")
        {
            ChangeVolume(0, "Effects");
            textChild.text = "MUTE";
        }
        else
        {
            ChangeVolume(1f, "Effects");
            textChild.text = "ON";
        }
    }
    void AddEffectsVol(Image g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("EffectsVolume") + 5f;

        if (aux > 100)
            aux = 100;

        ChangeVolume(aux, "EffectsVolume");
    }

    void SubsEffectsVol(Image g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("EffectsVolume") - 5f;

        if (aux < 0)
            aux = 0;

        ChangeVolume(aux, "EffectsVolume");
    }


    void AddMusicVol(Image g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("MusicVolume") + 5f;

        if (aux > 100)
            aux = 100;

        ChangeVolume(aux, "MusicVolume");
    }

    void SubsMusicVol(Image g)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>("MusicVolume") - 5f;

        if (aux < 0)
            aux = 0;

        ChangeVolume(aux, "MusicVolume");
    }


}
