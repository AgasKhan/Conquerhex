using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFunctions : MonoBehaviour
{
    MenuManager refMenu;
    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(LoadButtons);
    }
    private void Start()
    {
        refMenu = MenuManager.instance;
    }

    void LoadButtons()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            // Static Buttons
            {"Button", Example},

            //Menu principal
            {"StartGame", StartGame},
            {"Dungeons", Dungeons},
            {"LoadGame", LoadGame},
            {"Settings", DisplayWindow},
            {"Store", ShowStore},
            {"Credits", DisplayWindow},
            {"DeleteWindow", DisplayWindow},
            {"DeleteData", DeleteData},
            {"Exit", Exit},
            {"Quit", DisplayWindow},
            

            //Menu de opciones
            {"MuteEffects", MuteEffects},
            {"MuteMusic", MuteMusic},
            

            //Menu de creacion de minions
            {"MBody", MBody},
            {"MArms", MArms},
            {"MLegs", MLegs},
            {"MHeads", MHeads},
            {"MTails", MTails},
            {"QUIT", QUIT},

            //Manejo de ventanas
            {"QuitWindow", QuitWindow},
            {"CloseWindow", CloseWindow},
            {"Panel1", DisplayWindow},
            {"Panel2", DisplayWindow},
            {"Panel3", DisplayWindow},
            {"Details", DisplayWindow},
            
            //Menu in game
            {"Resume", Resume},
            {"Restart", Restart},
            
            {"Settings", DisplayWindow},
            {"BackMenu", BackMenu},
            {"SaveGame", SaveGame},
            {"MenuInGame", PauseMenu},
            {"BuySingleItem", BuySingleItem},
            {"EquipItem", EquipItem},

            {"ItemsButtons_1", DisplayStore},
            {"ItemsButtons_2", DisplayStore},


            //Dragable Buttons
            {"ShowWindow", ShowWindow},

            //Tutorial
            {"ShowControls", ShowControls},

            //Display Items Special Actions 
            {"RefreshButton", RefreshMyButton}

        });
    }
    void DeleteData(GameObject g)
    {
        SaveWithJSON.DeleteData();
        Restart(g);
        ChangeVolume(1f, "Effects");
        ChangeVolume(1f, "Music");
    }

    void MuteEffects(GameObject g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = g.transform.GetComponent<UnityEngine.UI.Image>();

        if (textChild.text == "ON")
        {
            ChangeVolume(0, "Effects");
            textChild.text = "MUTE";
            imageChild.color = Color.gray;

            SaveWithJSON.SaveInPictionary(g.name, false);
        }
        else
        {
            ChangeVolume(1f, "Effects");
            textChild.text = "ON";
            imageChild.color = Color.white;

            SaveWithJSON.SaveInPictionary(g.name, true);
        }
    }
    void MuteMusic(GameObject g)
    {
        var textChild = g.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = g.transform.GetComponent<UnityEngine.UI.Image>();

        if (textChild.text == "ON")
        {
            ChangeVolume(0, "Music");
            textChild.text = "MUTE";
            imageChild.color = Color.gray;
            refMenu.MuteCurrentMusic(false);

            SaveWithJSON.SaveInPictionary(g.name, false);
        }
        else
        {
            ChangeVolume(1f, "Music");
            textChild.text = "ON";
            imageChild.color = Color.white;
            refMenu.MuteCurrentMusic(true);

            SaveWithJSON.SaveInPictionary(g.name, true);
        }
    }

    void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;

        if(name == "Music")
            refMenu.music.audioMixer.SetFloat(name, value);
        else
            refMenu.effects.audioMixer.SetFloat(name, value);
    }


    void ShowControls(GameObject g)
    {
        DisplayWindow(g);
        //refMenu.MusicInGame(false);
        //refMenu.PlayTutorialM(true);
    }

    void RefreshMyButton(GameObject g)
    {
        var aux = g.GetComponent<DisplayItem>();
        aux.myDetailWindow.RefreshButton(aux.myItem.nameDisplay);
    }

    void DisplayStore(GameObject g)
    {
        refMenu.CloseLastWindow(g.transform.parent.parent.parent.parent);
        refMenu.ShowWindow(g.name);
    }
    
    void ShowStore(GameObject g)
    {
        DisplayWindow(g);
        Store.instance.RefreshPlayerCoins();
    }



    void BuySingleItem(GameObject g)
    {
        if (Store.instance.BuyAnItem(g.transform.parent.name))
        {
            var aux = g.GetComponent<UnityEngine.UI.Button>();
            aux.interactable = false;

            BaseData.playerInventory.Add(g.transform.parent.name);
            
            DetailsWindow.instance.EnableButton();
            Store.instance.RefreshPlayerCoins();
        }
    }

    void EquipItem(GameObject g)
    {
        //var character = GameManager.instance.player.GetComponent<Character>();

        Debug.Log("El jugador se equipó: " + g.transform.parent.name);

        BaseData.currentWeapon = g.transform.parent.name;

        Debug.Log("Current weapon = " + BaseData.currentWeapon);

        var aux = g.GetComponent<UnityEngine.UI.Button>();
        aux.interactable = false;


        //------------------------------------------------------------------------
        /*
        var body = Manager<ItemBase>.pic["Pj"] as BodyBase;
        var weapon = Manager<ItemBase>.pic[g.transform.parent.name] as WeaponBase;

        body.principal.weapon = weapon;
        */
        //------------------------------------------------------------------------
    }

    void LoadGame(GameObject g)
    {

    }

    void Dungeons(GameObject g)
    {

    }

    void PauseMenu(GameObject g)
    {
        DisplayWindow(g);
        GameManager.instance.TogglePause();
        //Despausar
    }

    void Resume(GameObject g)
    {
        refMenu.CloseLastWindow();
        GameManager.instance.TogglePause();
    }
    void Restart(GameObject g)
    {
        LoadSystem.instance.Reload();
        refMenu.CloseLastWindow();
    }
    void BackMenu(GameObject g)
    {
        LoadSystem.instance.Load("MainMenu");

        if (Application.platform == RuntimePlatform.Android)
            SaveWithJSON.SaveGameAndroid();
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
            SaveWithJSON.SaveGameWindows();
    }
    void SaveGame(GameObject g)
    {

    }

    void QuitWindow (GameObject g)
    {
        
    }

    void CloseWindow (GameObject g)
    {
        refMenu.CloseLastWindow();
    }




    #region Static Buttons
    void Example(GameObject g)
    {
        Debug.Log("Apretaste el boton");
    }

    void StartGame(GameObject g)
    {
        refMenu.StartGame();
        refMenu.StartSound();
    }

    void DisplayWindow(GameObject g)
    {
        refMenu.ShowWindow(g.name);
    }

    void Exit(GameObject g)
    {
        SaveWithJSON.SaveGameAndroid();
        Application.Quit();
    }



    void MBody(GameObject g)
    {
        
    }
    void MArms(GameObject g)
    {

    }
    void MLegs(GameObject g)
    {

    }
    void MHeads(GameObject g)
    {

    }
    void MTails(GameObject g)
    {

    }
    void QUIT(GameObject g)
    {

    }

    #endregion

    #region Drageable Buttons
    void ShowWindow(GameObject g)
    {
        Debug.Log("Apretaste un boton Draggeable");
        //DetailsWindow.instance.SetWindow(_mySprite, _information);
    }

    #endregion

    #region FuncionesAntiguas
    /*
    // Llamada cada que cambia el slider
    void CameraSpeed(GameObject g, float f)
    {
        // Almacenar 
        //CSVReader.SaveInPictionary("MouseSensibility", f);
        
        print("Todo funciono: " + f);
        if (player == null)
            return;


        player.cameraScript.Speed(f);
    }

    // On
    
    void CameraSpeed(Slider s)
    {
        s.value = CSVReader.LoadFromPictionary<float>("MouseSensibility", 25);
    }

    void ChangeAlphaReticula(GameObject g, float f)
    {
        CSVReader.SaveInPictionary("AlphaReticula", f);
        MainHud.ReticulaAlpha(f);
    }

    void OnAlphaReticula(Slider s)
    {
        s.value = CSVReader.LoadFromPictionary<float>("AlphaReticula", 0.15f);
    }
    

    void ChangeVolumeLevel(GameObject g, float volume)
    {
        ChangeVolume(volume, g.name);
    }

    float LoadVolume(string name)
    {
        float volume;
        group.audioMixer.GetFloat(name, out volume);
        return Mathf.Pow(10, (volume / 20));
    }


    void LoadVolumeLevel(Slider s)
    {
        s.value = LoadVolume(s.name);
    }

    void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;
        group.audioMixer.SetFloat(name, value);
    }

    void Mute(GameObject g)
    {
        //menu.eventListFloat["Master"](0.01f);
        if (ChangeText(g))
            ChangeVolume(1f, "Master");
        else
            ChangeVolume(0, "Master");

    }

    //////////////////////////////////
    ///hacer herencia con configuration
    ///
    bool ChangeText(GameObject g)
    {
        TMPro.TextMeshProUGUI text = g.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        //text.text = (text.text == "Activate") ? "Deactivate" : "Activate";

        if ((text.text == "Activate"))
        {
            text.text = "Deactivate";
            return false;
        }
        else
        {
            text.text = "Activate";
            return true;
        }

    }

    void ChangeText(GameObject g, bool active)
    {
        TMPro.TextMeshProUGUI text = g.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        text.text = active ? "Deactivate" : "Activate";
    }
    */

    #endregion
}
