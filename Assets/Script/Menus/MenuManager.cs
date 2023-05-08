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

        LoadSystem.AddPostLoadCorutine(PLayAudios);
    }

    void PLayAudios()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            MusicInGame(true);
        }
        else
        {
            GameMusic();
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

    public void ClickSound()
    {
        audioM.Play("Click");
    }
    public void MusicInGame(bool condition)
    {
        if(condition == true)
            audioM.Play("MusicInMenu");
        else
            audioM.Stop("MusicInMenu");
    }

    public void GameMusic()
    {
        audioM.Play("GameMusic");
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
            audioM.Stop("TutorialMusic");
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