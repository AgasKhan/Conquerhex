using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonsFunc_Menu : ButtonsFunctions
{
    protected override void LoadButtons()
    {
        base.LoadButtons();

        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            //Opciones principales
            {"StartGame", StartGame},
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
        refMenu.StartSound();
    }

    void Dungeons(GameObject g)
    {

    }

    void LoadGame(GameObject g)
    {

    }

    void ShowSettings(GameObject g)
    {
        CreateSubMenu.CreateNavBar((submenu) => { submenu.AddNavBarButton("Sounds", "Sounds"); });
        CreateSubMenu.CreateBody(BodyCreateSettings);
    }

    void Credits (GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
           .SetWindow("Creditos:", "Lucas Galardo"+ "\n" + "karina Revelli" + "\n" + "Jhamil Castillo"+ "\n" + "Lider Huamantuco")
           .AddButton("Close", () => {

               refMenu.modulesMenu.ObtainMenu<PopUp>(false);
           });

    }

    void DeleteData(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
            .SetWindow("Borrar partida", "¿Estas seguro que deseas borrar toda la configuracion y progreso?")
            .AddButton("Confirmar", () => {

                StoreInteract.instance.ClearCustomerInventory();
                SaveWithJSON.DeleteData();
                //Restart(g);

            })
            .AddButton("Cancelar", () => {

                refMenu.modulesMenu.ObtainMenu<PopUp>(false);
            });
        //refMenu.ChangeVolume(1f, "Effects");
        //refMenu.ChangeVolume(1f, "Music");
    }

    void Exit(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
            .SetWindow("Salir del Juego", "¿Estas seguro que deseas Salir?")
            .AddButton("Confirmar", () => {

                SaveWithJSON.SaveGame();
                Application.Quit();
            })
            .AddButton("Cancelar", () => {

                refMenu.modulesMenu.ObtainMenu<PopUp>(false);

             });
    }

    void BodyCreateSettings(SubMenus submenu)
    {
        submenu.CreateSection(0, 6);

            submenu.AddComponent<PopUp>().SetWindow("Title", "Description");

    }
}
