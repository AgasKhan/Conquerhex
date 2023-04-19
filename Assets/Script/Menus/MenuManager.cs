using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    string firstLevel;

    [HideInInspector]
    public SceneChanger refSceneChanger;
    //public AudioManager audioM;


    //para los eventos-------------------------------------------------------
    public Pictionarys<string, Action<GameObject>> eventListVoid = new Pictionarys<string, Action<GameObject>>();
    public Pictionarys<string, Action<GameObject, float>> eventListFloat = new Pictionarys<string, Action<GameObject, float>>();
    public Pictionarys<string, Action<GameObject, string>> eventListString = new Pictionarys<string, Action<GameObject, string>>();

    //public Pictionarys<string, Action<Slider>> eventListSliderOn = new Pictionarys<string, Action<Slider>>();
    public Pictionarys<string, Action<Button>> eventListButtoOn = new Pictionarys<string, Action<Button>>();
    //-----------------------------------------------------------------------


    public Pictionarys<string, GameObject> subMenus = new Pictionarys<string, GameObject>();
    public Pictionarys<string, DetailsWindow> detailsWindows = new Pictionarys<string, DetailsWindow>();

    protected override void Awake()
    {
        base.Awake();

        refSceneChanger = GetComponent<SceneChanger>();
        //audioM = GameManager.GetComponent<AudioManager>();

        GetDetailsWinAndSubMenus();
    }

    void GetDetailsWinAndSubMenus()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject aux = transform.GetChild(i).gameObject;
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

    public void StartGame()
    {
        //ClickAccept();
        refSceneChanger.Load(firstLevel);
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