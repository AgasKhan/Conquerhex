using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonsFunc_Menu : ButtonsFunctions
{
    [SerializeField]
    ShowSubMenuSettings ShowSubMenuSettings;

    protected override void LoadButtons()
    {
        base.LoadButtons();

        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            //Opciones principales
            {"StartGame", StartGame},
            {"Tutorial", Tutorial},
            {"Dungeons", Dungeons},
            {"LoadGame", LoadGame},
            //{"Settings", Settings},
            {"Credits", Credits},
            {"DeleteWindow", DeleteData},
            {"Exit", Exit},
            {"ShowSettings", ShowSettings}
            //{"Quit", DisplayWindow},
        });
    }

    void StartGame(GameObject g)
    {
        refMenu.StartGame();
    }

    void Tutorial(GameObject g)
    {
        GameManager.instance.Load("Tutorial_Fixed");
    }

    void Dungeons(GameObject g)
    {

    }

    void LoadGame(GameObject g)
    {

    }

    void ShowSettings(GameObject g)
    {
        //CreateSubMenu.CreateNavBar((submenu) => { submenu.AddNavBarButton("Sounds", "Sounds"); });
        //CreateSubMenu.CreateBody(BodyCreateSettings);
        ShowSubMenuSettings.Create();
    }

    void Credits (GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
           .SetWindow("Creditos:", "Lucas Galardo"+ "\n" + "Karina Revelli" + "\n" + "Jhamil Castillo")
           .AddButton("Cerrar", () => 
           {
               refMenu.modulesMenu.ObtainMenu<PopUp>(false);
               GameManager.instance.Menu(false);
           });

    }

    void DeleteData(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
            .SetWindow("Borrar partida", "¿Estas seguro que deseas borrar todo tu progreso?")
            .AddButton("Confirmar", () => 
            {

                //BuildingInteract.instance.ClearCustomerInventory();
                SaveWithJSON.DeleteData();
                Restart(g);

            })
            .AddButton("Cancelar", () => 
            {

                refMenu.modulesMenu.ObtainMenu<PopUp>(false);
                GameManager.instance.Menu(false);
            });
        //refMenu.ChangeVolume(1f, "Effects");
        //refMenu.ChangeVolume(1f, "Music");
    }

    void Exit(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
            .SetWindow("Salir del Juego", "¿Estas seguro que deseas Salir?")
            .AddButton("Confirmar", () => 
            {

                SaveWithJSON.SaveGame();
                Application.Quit();
            })
            .AddButton("Cancelar", () => 
            {

                refMenu.modulesMenu.ObtainMenu<PopUp>(false);
                GameManager.instance.Menu(false);
            });
    }

}
