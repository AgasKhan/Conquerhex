using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;


public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    string firstLevel;

    public AudioManager audioM;
    public AudioMixerGroup effects;
    public AudioMixerGroup music;

    //para los eventos-------------------------------------------------------
    public Pictionarys<string, Action<GameObject>> eventListVoid = new Pictionarys<string, Action<GameObject>>();
    public Pictionarys<string, Action<GameObject, float>> eventListFloat = new Pictionarys<string, Action<GameObject, float>>();
    public Pictionarys<string, Action<GameObject, string>> eventListString = new Pictionarys<string, Action<GameObject, string>>();

    //public Pictionarys<string, Action<Slider>> eventListSliderOn = new Pictionarys<string, Action<Slider>>();
    public Pictionarys<string, Action<Button>> eventListButtoOn = new Pictionarys<string, Action<Button>>();
    //-----------------------------------------------------------------------


    //[SerializeField]
    public Pictionarys<string, GameObject> subMenus = new Pictionarys<string, GameObject>();

    //[SerializeField]
    Pictionarys<string, DetailsWindow> detailsWindows;

    protected override void Awake()
    {
        base.Awake();

        audioM = GetComponent<AudioManager>();

        detailsWindows = Manager<DetailsWindow>.pic;

        GetDetailsWinAndSubMenus();

        LoadSystem.AddPostLoadCorutine(InitScenes);
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
        {
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("MusicVolume"), "Music");
        }
        else if (SaveWithJSON.CheckKeyInBD("EffectsVolume"))
        {
            ChangeVolume(SaveWithJSON.LoadFromPictionary<float>("EffectsVolume"), "Effects");
        }
    }

    void FirstStart()
    {
        if(!SaveWithJSON.CheckKeyInBD("ShowTutorial"))
        {
            SaveWithJSON.SaveInPictionary("ShowTutorial", false);
            ShowWindow("ShowControls");
        }
    }

    void GetDetailsWinAndSubMenus()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject aux = transform.GetChild(i).gameObject;

            for (int j = 0; j < aux.transform.childCount; j++)
            {
                GameObject aux2 = aux.transform.GetChild(j).gameObject;

                var detailsWin2 = aux2.GetComponent<DetailsWindow>();

                if (detailsWin2 != null)
                    detailsWindows.Add(aux2.name, detailsWin2);
            }

            var detailsW= aux.GetComponent<DetailsWindow>();

            if (detailsW != null)
                detailsWindows.Add(aux.name, detailsW);
            else
                subMenus.Add(aux.name, aux);
        }
    }

    public void ShowWindow(string key)
    {
        if (subMenus.ContainsKey(key))
        {
            subMenus[key].SetActive(true);
            subMenus[key].transform.SetAsLastSibling();
        }
        else if (detailsWindows.ContainsKey(key))
        {
            detailsWindows[key].ShowOrHide(true);
            detailsWindows[key].transform.SetAsLastSibling();
        }
        else
            Debug.Log("No se encontro: " + key + " en los pictionarys");
    }



    public void CloseLastWindow()
    {
        var lastChild = transform.GetChild(transform.childCount - 1);
        var aux = lastChild.GetComponent<DetailsWindow>();

        if (aux != null)
            aux.ShowOrHide(false);
        else
            lastChild.gameObject.SetActive(false);

        lastChild.SetAsFirstSibling();
    }
    public void CloseLastWindow(Transform tr)
    {
        var lastChild = tr.GetChild(tr.childCount - 1);
        var aux = lastChild.GetComponent<DetailsWindow>();

        if (aux != null)
            aux.ShowOrHide(false);
        else
            lastChild.gameObject.SetActive(false);

        lastChild.SetAsFirstSibling();
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