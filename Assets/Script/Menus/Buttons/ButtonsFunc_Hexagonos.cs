using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFunc_Hexagonos : ButtonsFunctions
{
    [SerializeField]
    ShowSubMenuControls showSubMenuControls;
    
    [SerializeField]
    StatisticsSubMenu statisticsSubMenu;

    //[SerializeField]
    //BuildingsSubMenu buildingsSubMenu;
    protected override void LoadButtons()
    {
        base.LoadButtons();

        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {
            //In Game
            {"MenuInGame", PauseMenu},
            {"Try1", Try1},
            {"Try2", Try2},

            //Menu in game
            {"Resume", Resume},
            {"Restart", Restart},
            {"ShowControls", ShowControls},
            {"BackMenu", BackMenu},
        });
    }

    void PauseMenu(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<MenuList>(false).SetActiveGameObject(true).CreateDefault();
        GameManager.instance.Pause(true);
        //Despausar
    }

    void Try1(GameObject g)
    {
        statisticsSubMenu.Create();
    }

    void Try2(GameObject g)
    {
        //buildingsSubMenu.Create();
    }

    void Resume(GameObject g)
    {
        refMenu.modulesMenu.ObtainMenu<MenuList>(false);
        GameManager.instance.Pause(false);
    }

    void BackMenu(GameObject g)
    {
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "¿Seguro deseas volver al menu?")
                .AddButton("Si", () => { LoadSystem.instance.Load("MainMenu");})
                .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    void ShowControls(GameObject g)
    {
        showSubMenuControls.Create();
    }
}
