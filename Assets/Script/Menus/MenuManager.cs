using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class MenuManager : SingletonMono<MenuManager>
{
    [System.Serializable]
    public struct SelectLevels
    {
        public DoubleString texts;
        public Sprite previewImage;
        public int numberScene;
    }

    [SerializeField]
    DetailsWindow[] AllDetailsWindows;

    public SelectLevels[] preview;

    public GameObject[] menus;
    public GameObject[] subMenus;
    public Button[] levelButtons;
    public string firstLevel;
    public GameObject gamePausedMenu;
    public SceneChanger refSceneChanger;
    public AudioManager audioM;

    private int _currentMemuPrincipal = 0;
    private int _currentMemu = 0;
    //private int _currentPreview = 0;

    //private string  _currentSubMemu= "GeneralOptionsButton";
    private bool _inGame = true;

    //para los eventos
    public Pictionarys<string, Action<GameObject>> eventListVoid = new Pictionarys<string, Action<GameObject>>();
    public Pictionarys<string, Action<GameObject, float>> eventListFloat = new Pictionarys<string, Action<GameObject, float>>();
    public Pictionarys<string, Action<GameObject, string>> eventListString = new Pictionarys<string, Action<GameObject, string>>();

    public Pictionarys<string, Action<Slider>> eventListSliderOn = new Pictionarys<string, Action<Slider>>();
    public Pictionarys<string, Action<Button>> eventListButtoOn = new Pictionarys<string, Action<Button>>();

    /*
    private void Awake()
    {
        instance = this;

        
         
        if (levelButtons != null)
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int number = i + 1;
                TextMeshProUGUI aux = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                if (aux != null)
                {
                    aux.text = "Play Level  " + number.ToString();

                    levelButtons[i].onClick.RemoveAllListeners();
                    levelButtons[i].onClick.AddListener(() =>  //Funcion Lambda
                    {
                        SelectLevel(number.ToString());
                    });
                }
            }

        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "Lobby")
        {
            _inGame = true;
        }

        else
        {
            _inGame = false;
            Cursor.lockState = CursorLockMode.None;
        }

        audioM = GetComponent<AudioManager>();

        
    }
    */
    protected override void Awake()
    {
        base.Awake();

        if (AllDetailsWindows != null)
        {
            for (int i = 0; i < AllDetailsWindows.Length; i++)
            {
                Manager<DetailsWindow>.pic.Add(AllDetailsWindows[i].transform.name, AllDetailsWindows[i]);
            }
        }

        refSceneChanger = GetComponent<SceneChanger>();
    }

    private void Start()
    {
        /*
        if (!_inGame)
        {
            audioM.Play("MenuMusic");
        }
        */
    }

    void Update()
    {
        if (_inGame)
        {
            /*
             
            if (Controllers.pause.down)
            {
                OpenCloseMenu();
            }

            
            if (_optionMenuActive)
            {
                if (Input.GetKeyDown("h"))
                    GoToPreviousSubMenu();

                if (Input.GetKeyDown("j"))
                    GoToNextSubMenu();
            }
            
            if (Controllers.locked.down)
                Controllers.MouseLock();
            
             */
        }

    }

    public void GoToNextSubMenu()
    {
        ClickSound();
        if (_currentMemu < subMenus.Length - 1)
        {
            subMenus[_currentMemu].SetActive(false);
            _currentMemu++;
            subMenus[_currentMemu].SetActive(true);
        }
    }

    public void GoToPreviousSubMenu()
    {
        ClickSound();
        if (_currentMemu > 0)
        {
            subMenus[_currentMemu].SetActive(false);
            _currentMemu--;
            subMenus[_currentMemu].SetActive(true);
        }
    }
    public void ChangeMenu(int index)
    {
        ClickSound();
        if (index != _currentMemuPrincipal)
        {
            menus[_currentMemuPrincipal].SetActive(false);
            menus[index].SetActive(true);
            _currentMemuPrincipal = index;
        }
        DetailsWindow.ChangeAlpha(0, 0.1f);
    }


    public void ChangeSubMenu(int index)
    {
        ClickSound();
        if (index != _currentMemu)
        {
            subMenus[_currentMemu].SetActive(false);
            subMenus[index].SetActive(true);
            _currentMemu = index;
        }
    }

    public void OpenCloseMenu()
    {
        gamePausedMenu.SetActive(!gamePausedMenu.activeSelf);

        Time.timeScale = System.Convert.ToInt32(!gamePausedMenu.activeSelf);

        //Controllers.MouseLock(!gamePausedMenu.activeSelf);

        //GameManager.saveTime = !gamePausedMenu.activeSelf;

        //Controllers.verticalMouse.enable = !gamePausedMenu.activeSelf;
        //Controllers.horizontalMouse.enable = !gamePausedMenu.activeSelf;
    }

    public void StartGame()
    {
        //ClickAccept();
        refSceneChanger.Load(firstLevel);
    }

    public void SelectLevel(string number)
    {
        /*var aux = this.GetComponentInChildren<TMP_Text>();
        string level = "";
        if (aux!=null)
            level = "Level_" + aux.text;*/
        ClickAccept();
        string level = "Level_" + number;

        // if(level != null)
        //Application.loadedLevelName

        if (_inGame)
            //Controllers.eneable = false;


        refSceneChanger.Load(level);
    }

    public void CloseMainMenus()
    {
        ClickSound();
        menus[_currentMemuPrincipal].SetActive(false);
        menus[0].SetActive(true);
        _currentMemuPrincipal = 0;
    }

    public void ChangePreviews(int index)
    {
        ClickSound();

        DetailsWindow.ChangeAlpha(1, 0.1f);

        //DetailsWindow.PreviewImage(true, preview[index].previewImage);

        DetailsWindow.SetMyButton(() => { SelectLevel(preview[index].numberScene.ToString()); }, true, "Play: " + preview[index].texts.superior);

        DetailsWindow.ActiveButtons(false);

        var aux = preview[index].texts;

        aux.inferior += "\n" + "Misiones incompletas:".RichText("color", "red") + "\n";
        /*
        foreach (var item in Quests.SrchIncomplete(preview[index].numberScene))
        {
            aux.inferior += ("\n" + item.Description.superior.RichText("size", "16") + "\n" + item.Description.inferior.RichText("size", "12") + "\n");
        }

        aux.inferior += "\n" + "Misiones completas:".RichText("color", "green") + "\n";

        foreach (var item in Quests.SrchComplete(preview[index].numberScene))
        {
            aux.inferior += ("\n" + item.Description.superior.RichText("size", "16") + "\n" + item.Description.inferior.RichText("size", "12") + "\n");
        }
        */
        DetailsWindow.ModifyTexts(aux);
    }

    public void BackToLobby()
    {
        /*
        foreach (var item in Quests.SrchIncomplete(BaseData.currentLevel))
        {
            item.active = false;
        }

        refSceneChanger.Load("Lobby");
        */
    }


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