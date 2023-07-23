using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBuild : Building
{
    public Pictionarys<string, List<Item>> allInventories = new Pictionarys<string, List<Item>>();
    public override string rewardNextLevel => throw new System.NotImplementedException();
    public override void EnterBuild()
    {
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "¿Deseas guardar tu progreso?")
            .AddButton("Si", () => { SaveWithJSON.SaveInPictionary("PlayerInventory", character.inventory); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); SaveWithJSON.SaveGame(); })
            .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }
    
}
