using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class ShowSubMenuSettings : CreateSubMenu
{

    public AudioManager audioM;
    public AudioMixerGroup music;
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
                
            subMenu.AddComponent<ButtonB>().SetLeftButton(SubsMusicVol).SetRightButton(AddMusicVol);

        subMenu.CreateSection(3, 6);

            subMenu.AddComponent<DetailsWindow>().SetTexts("Effects Volumen", "");

            subMenu.AddComponent<ButtonB>().SetLeftButton(SubsEffectsVol).SetRightButton(AddEffectsVol);
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
        if (!SaveWithJSON.CheckKeyInBD("ShowTutorial"))
        {
            SaveWithJSON.SaveInPictionary("ShowTutorial", false);
            //subMenus.ShowWindow("ShowControls");
            //aqui se debe abrir el mostrar controles
        }
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
    void AddEffectsVol(Image g)
    {
        UpdateVol(g, "EffectsVolume", 0.1f);
    }

    void SubsEffectsVol(Image g)
    {
        UpdateVol(g, "EffectsVolume", -0.1f);
    }

    void AddMusicVol(Image g)
    {
        UpdateVol(g, "MusicVolume", 0.1f);
    }

    void SubsMusicVol(Image g)
    {
        UpdateVol(g, "MusicVolume", -0.1f);
    }

    void UpdateVol(Image g, string str, float number)
    {
        var aux = SaveWithJSON.LoadFromPictionary<float>(str) + number;

        if (aux < 0)
            aux = 0;
        else if (aux > 1)
            aux = 1;

        g.fillAmount = aux;

        ChangeVolume(aux, str);
    }

    void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;

        music.audioMixer.SetFloat(name, value);
        SaveWithJSON.SaveInPictionary(name, volume);
    }

}
