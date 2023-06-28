using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBuild : Building
{
    public Pictionarys<string, List<Item>> allInventories = new Pictionarys<string, List<Item>>();
    public override string rewardNextLevel => throw new System.NotImplementedException();
    //---------------------------------
    public MenuManager refMenu;
    //---------------------------------
    public override void EnterBuild()
    {
        //SaveWithJSON.SaveGame();
        //MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "Tu progreso ha sido guardado exitosamente").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "¿Deseas guardar tu progreso?")
            .AddButton("Si", () => { SaveWithJSON.SaveInPictionary("PlayerInventory", character.inventory); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); SaveWithJSON.SaveGame(); })
            .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

        
    }
    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        //---------------------------------
        refMenu.eventListVoid.Add("Try5", Try5);
        //---------------------------------
    }
    //---------------------------------
    void Try5(GameObject g)
    {
        myBuildSubMenu.Create();
    }
    //---------------------------------
}
