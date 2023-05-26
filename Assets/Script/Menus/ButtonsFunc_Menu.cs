using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            {"Store", ShowStore},
            {"Credits", DisplayWindow},
            {"DeleteWindow", DeleteData},
            {"Exit", Exit},
            {"Quit", DisplayWindow},

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

    void ShowStore(GameObject g)
    {
        DisplayWindow(g);
        //Building.instance.RefreshPlayerCoins();
    }

    void DeleteData(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<PopUp>(true)
            .SetWindow("Borrar partida", "¿Estas seguro que deseas borrar toda la configuracion y progreso?")
            .AddButton("Confirmar", () => {

                StoreInteract.instance.ClearCustomerInventory();
                SaveWithJSON.DeleteData();
                Restart(g);

            })
            .AddButton("Cancelar", () => {

                refMenu.modulesMenu.ObtainMenu<PopUp>(false);
            });


        //refMenu.ChangeVolume(1f, "Effects");
        //refMenu.ChangeVolume(1f, "Music");
    }

    void Exit(GameObject g)
    {
        SaveWithJSON.SaveGame();
        Application.Quit();
    }



}
