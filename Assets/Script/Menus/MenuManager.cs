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


    //[HideInInspector]
    private Pictionarys<string, GameObject> panels = new Pictionarys<string, GameObject>();

    private string lastPanel = "";

    protected override void Awake()
    {
        base.Awake();

        refSceneChanger = GetComponent<SceneChanger>();
        //audioM = GameManager.GetComponent<AudioManager>();

        GetDetailsWinAndPanels();
    }

    void GetDetailsWinAndPanels()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject aux = transform.GetChild(i).gameObject;
            var detailsW= aux.GetComponent<DetailsWindow>();

            if (detailsW != null)
                Manager<DetailsWindow>.pic.Add(aux.name, detailsW);
            else
                panels.Add(aux.name, aux);
        }
    }

    public void ShowPanel(string key)
    {
        if (panels.ContainsKey(key))
        {
            panels[key].SetActive(true);
            panels[key].transform.SetAsLastSibling();
            lastPanel = key;
        }
        else
            Debug.Log("No se encontro el panel: " + key);
    }

    public void CloseLastPanel()
    {
        if(lastPanel != "")
        {
            panels[lastPanel].SetActive(false);
            panels[lastPanel].transform.SetAsFirstSibling();

            lastPanel = transform.GetChild(transform.childCount - 1).name;
        }
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